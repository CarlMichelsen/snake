using Domain.Snake;

namespace Presentation.Snake;

public interface ILobbyContainer
{
    IReadOnlyDictionary<Guid, Lobby> Lobbies { get; }
    
    Lobby? AddLobby(Lobby lobby);
    
    bool RemoveLobby(Guid lobbyId);
}