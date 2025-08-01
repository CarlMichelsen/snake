using Presentation.Auth.Dto;

namespace Presentation.Hub.Lobby.Dto;

public record ChatMessageDto(
    Guid Id,
    UserDto Sender,
    string Content,
    DateTimeOffset TimeStamp);