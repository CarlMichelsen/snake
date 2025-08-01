namespace Presentation.Auth.Dto;

public record UserDto(
    Guid Id,
    string Name,
    DateTimeOffset CreatedAt);