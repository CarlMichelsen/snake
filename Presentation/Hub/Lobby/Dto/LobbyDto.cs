using Presentation.Auth.Dto;

namespace Presentation.Hub.Lobby.Dto;

public record LobbyDto(
    Guid Id,
    UserDto LobbyLeader,
    List<ChatMessageDto> Messages,
    Dictionary<Guid, UserDto> Users);