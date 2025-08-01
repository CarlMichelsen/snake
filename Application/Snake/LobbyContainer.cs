using Domain.Snake;
using Presentation.Snake;

namespace Application.Snake;

public class LobbyContainer : ILobbyContainer
{
    private readonly Dictionary<Guid, Lobby> lobbies = [];

    public IReadOnlyDictionary<Guid, Lobby> Lobbies => this.lobbies;
    
    public Lobby? AddLobby(Lobby lobby)
    {
        return this.lobbies.TryAdd(lobby.Id, lobby)
            ? lobby
            : null;
    }

    public bool RemoveLobby(Guid lobbyId)
    {
        return this.lobbies.Remove(lobbyId);
    }
}