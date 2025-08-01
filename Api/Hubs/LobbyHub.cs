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
    
    public override Task OnConnectedAsync()
    {
        var session = simpleLogin.GetSession()!;
        
        this.Context.Items[SessionKey] = session;
        logger.LogInformation(
            "OnConnectedAsync {Username}",
            this.Session.User.Name);
        return base.OnConnectedAsync();
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

    public Task<LobbyDto?> CreateLobby()
    {
        var lobby = lobbyContainer.AddLobby(new Lobby(this.Session.User, timeProvider));
        if (lobby is null)
        {
            return Task.FromResult<LobbyDto?>(null);
        }
        
        return Task.FromResult<LobbyDto?>(lobby.ToDto());
    }

    public Task<LobbyDto?> JoinLobby(Guid lobbyId)
    {
        if (!lobbyContainer.Lobbies.TryGetValue(lobbyId, out var lobby))
        {
            return Task.FromResult<LobbyDto?>(null);
        }
        
        lobby.Join(this.Session.User);
        this.Session.Lobby = lobby;
        
        this.LobbyGroup?.UserJoined(this.Session.User.ToDto());
        this.Groups.AddToGroupAsync(this.Context.ConnectionId, lobbyId.ToString());
        
        // Sync messages
        this.Clients.Caller.SetMessages(lobby.Messages.Select(m => m.ToDto()).ToList());

        return Task.FromResult<LobbyDto?>(lobby.ToDto());
    }

    public Task LeaveCurrentLobby()
    {
        if (this.Session.Lobby is null)
        {
            logger.LogWarning("Attempted to leave current lobby without being in a lobby");
            return Task.CompletedTask;
        }
        
        if (!lobbyContainer.Lobbies.TryGetValue(this.Session.Lobby.Id, out var lobby))
        {
            logger.LogWarning("Attempted to leave current lobby with invalid lobby");
            return Task.CompletedTask;
        }
        
        lobby.Kick(this.Session.User);
        this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId, lobby.Id.ToString());
        
        this.LobbyGroup?.UserLeft(this.Session.User.ToDto());
        if (lobby.Users.Count == 0)
        {
            lobbyContainer.RemoveLobby(lobby.Id);
        }
        
        this.Session.Lobby = null;
        return Task.CompletedTask;
    }

    public Task SendMessage(string content)
    {
        if (this.Session.Lobby is null)
        {
            return Task.CompletedTask;
        }

        var message = this.Session.Lobby
            .SendMessage(this.Session.User.Id, content);
        
        this.LobbyGroup?.ReceiveMessage(message.ToDto());
        return Task.CompletedTask;
    }
}