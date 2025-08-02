using Presentation.Auth.Dto;
using Presentation.Hub.Lobby.Dto;

namespace Presentation.Hub.Lobby;

public interface ILobbyClientMethods
{
    Task SetLobby(LobbyDto? lobby);
    
    Task ReceiveMessage(ChatMessageDto chatMessage);
    
    Task SetMessages(List<ChatMessageDto> messages);
    
    Task UserJoined(UserDto user);
    
    Task UserActive(Guid userId, bool userActive);
    
    Task UserLeft(UserDto user);
}