namespace Presentation.Hub.Lobby.Dto;

public record LobbyDto(
    Guid Id,
    Guid LobbyLeaderId,
    List<ChatMessageDto> Messages,
    Dictionary<Guid, UserConnectionDto> Users);