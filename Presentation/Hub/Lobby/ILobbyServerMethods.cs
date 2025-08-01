using Presentation.Hub.Lobby.Dto;

namespace Presentation.Hub.Lobby;

public interface ILobbyServerMethods
{
    Task<LobbyDto?> CreateLobby();
    
    Task<LobbyDto?> JoinLobby(Guid lobbyId);
    
    Task LeaveCurrentLobby();
    
    Task SendMessage(string content);
}