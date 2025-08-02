using Application.Mappers;
using Domain.Snake;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Presentation.Auth;
using Presentation.Hub.Lobby;
using Presentation.Hub.Lobby.Dto;
using Presentation.Snake;

namespace Api.Hubs;

[Authorize]
public class LobbyHub(
    ILogger<LobbyHub> logger,
    TimeProvider timeProvider,
    ISimpleLogin simpleLogin,
    ILobbyContainer lobbyContainer)
    : Hub<ILobbyClientMethods>, ILobbyServerMethods
{
    private const string SessionKey = "session";
    
    private Session Session => this.Context.Items[SessionKey] as Session 
                               ?? throw new NullReferenceException("Session does not exist in Context.Items");

    private ILobbyClientMethods? LobbyGroup => this.Session.Lobby is not null
        ? this.Clients.Groups(this.Session.Lobby.Id.ToString())
        : null;
    
    public override async Task OnConnectedAsync()
    {
        var session = simpleLogin.GetSession()!;
        this.Context.Items[SessionKey] = session;

        if (this.Session.Lobby is not null)
        {
            await this.Groups.AddToGroupAsync(
                this.Context.ConnectionId,
                this.Session.Lobby.Id.ToString());
            await this.Clients.Caller.SetLobby(this.Session.Lobby.ToDto());
            
            this.Session.Lobby.Users[this.Session.User.Id].Active = true;
            await this.LobbyGroup!.UserActive(this.Session.User.Id, true);
        }
        
        logger.LogInformation(
            "OnConnectedAsync {Username}",
            this.Session.User.Name);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        logger.LogInformation(
            "OnDisconnectedAsync {Username}",
            this.Session.User.Name);
        if (exception is not null)
        {
            logger.LogError(
                exception,
                "OnDisconnectedAsync");
        }

        if (this.Session.Lobby is not null)
        {
            this.Session.Lobby.Users[this.Session.User.Id].Active = false;
            await this.LobbyGroup!.UserActive(this.Session.User.Id, false);
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task<LobbyDto?> CreateLobby()
    {
        if (this.Session.Lobby is not null)
        {
            await this.LeaveLobby();
        }
        
        var lobby = lobbyContainer.AddLobby(new Lobby(new UserConnection { Active = true, User = this.Session.User }, timeProvider));
        if (lobby is null)
        {
            return null;
        }
        
        this.Session.Lobby = lobby;
        
        logger.LogInformation("CreateLobby {Username}", this.Session.User.Name);
        var lobbyDto = lobby.ToDto();
        await this.Clients.Caller.SetLobby(lobbyDto);
        await this.Groups.AddToGroupAsync(
            this.Context.ConnectionId,
            this.Session.Lobby.Id.ToString());
        return lobbyDto;
    }

    public async Task<LobbyDto?> JoinLobby(Guid lobbyId)
    {
        if (this.Session.Lobby is not null)
        {
            if (lobbyId != this.Session.Lobby.Id)
            {
                await this.LeaveCurrentLobby();
            }
            else
            {
                var existingLobbyDto = this.Session.Lobby.ToDto();
                await this.Clients.Caller.SetLobby(existingLobbyDto);
                return existingLobbyDto;
            }
        }
        
        if (!lobbyContainer.Lobbies.TryGetValue(lobbyId, out var lobby))
        {
            return null;
        }
        
        lobby.Join(this.Session.User);
        this.Session.Lobby = lobby;
        
        lobby.Users[this.Session.User.Id].Active = true;
        await this.LobbyGroup!.UserActive(this.Session.User.Id, true);
        
        this.LobbyGroup?.UserJoined(this.Session.User.ToDto());
        await this.Groups.AddToGroupAsync(this.Context.ConnectionId, lobbyId.ToString());
        
        // Sync messages
        await this.Clients.Caller.SetMessages(lobby.Messages.Select(m => m.ToDto()).ToList());
        
        logger.LogInformation("JoinLobby {Username}", this.Session.User.Name);
        
        var lobbyDto = lobby.ToDto();
        await this.Clients.Caller.SetLobby(lobbyDto);
        return lobbyDto;
    }

    public async Task LeaveCurrentLobby()
    {
        await this.LeaveLobby();
    }

    public async Task SendMessage(string content)
    {
        if (this.Session.Lobby is null)
        {
            return;
        }

        var message = this.Session.Lobby
            .SendMessage(this.Session.User.Id, content);

        if (this.LobbyGroup is not null)
        {
            await this.LobbyGroup.ReceiveMessage(message.ToDto());
        }
    }
    
    private async Task LeaveLobby()
    {
        if (this.Session.Lobby is null)
        {
            logger.LogWarning("Attempted to leave current lobby without being in a lobby");
            return;
        }
        
        if (!lobbyContainer.Lobbies.TryGetValue(this.Session.Lobby.Id, out var lobby))
        {
            logger.LogWarning("Attempted to leave current lobby with invalid lobby");
            return;
        }
        
        lobby.Kick(this.Session.User);
        await this.Groups.RemoveFromGroupAsync(
            this.Context.ConnectionId, 
            lobby.Id.ToString());

        if (this.LobbyGroup is not null)
        {
            await this.LobbyGroup.UserLeft(this.Session.User.ToDto());
        }
        
        logger.LogInformation("LeaveCurrentLobby {Username}", this.Session.User.Name);
        this.Session.Lobby = null;
        await this.Clients.Caller.SetLobby(null);
        
        if (lobby.Users.Count == 0)
        {
            lobbyContainer.RemoveLobby(lobby.Id);
            logger.LogInformation("Lobby {LobbyId} was removed because everyone left it", lobby.Id);
        }
    }
}