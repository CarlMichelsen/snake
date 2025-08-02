using Presentation.Auth.Dto;

namespace Presentation.Hub.Lobby.Dto;

public record UserConnectionDto(
    bool Active,
    UserDto User);