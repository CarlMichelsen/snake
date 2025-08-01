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
    
    public Session Session => this.Context.Items[SessionKey] as Session 
                              ?? throw new NullReferenceException("Session does not exist in Context.Items");

    public ILobbyClientMethods? LobbyGroup => this.Session.Lobby is not null
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
        }
        
        logger.LogInformation(
            "OnConnectedAsync {Username}",
            this.Session.User.Name);
        await base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
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
        
        return base.OnDisconnectedAsync(exception);
    }

    public async Task<LobbyDto?> CreateLobby()
    {
        if (this.Session.Lobby is not null)
        {
            await this.LeaveLobby();
        }
        
        var lobby = lobbyContainer.AddLobby(new Lobby(this.Session.User, timeProvider));
        if (lobby is null)
        {
            return null;
        }
        
        this.Session.Lobby = lobby;
        
        logger.LogInformation("CreateLobby {Username}", this.Session.User.Name);
        return lobby.ToDto();
    }

    public async Task<LobbyDto?> JoinLobby(Guid lobbyId)
    {
        if (this.Session.Lobby is not null)
        {
            await this.LeaveCurrentLobby();
        }
        
        if (!lobbyContainer.Lobbies.TryGetValue(lobbyId, out var lobby))
        {
            return null;
        }
        
        lobby.Join(this.Session.User);
        this.Session.Lobby = lobby;
        
        this.LobbyGroup?.UserJoined(this.Session.User.ToDto());
        await this.Groups.AddToGroupAsync(this.Context.ConnectionId, lobbyId.ToString());
        
        // Sync messages
        await this.Clients.Caller.SetMessages(lobby.Messages.Select(m => m.ToDto()).ToList());
        
        logger.LogInformation("JoinLobby {Username}", this.Session.User.Name);

        return lobby.ToDto();
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
        
        if (lobby.Users.Count == 0)
        {
            lobbyContainer.RemoveLobby(lobby.Id);
        }
        
        logger.LogInformation("LeaveCurrentLobby {Username}", this.Session.User.Name);
        this.Session.Lobby = null;
    }
}