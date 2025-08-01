using Domain;
using Presentation.Auth.Dto;

namespace Application.Mappers;

public static class UserMapper
{
    public static UserDto ToDto(this User user)
        => new UserDto(user.Id, user.Name, user.CreatedAt);
    
    public static User ToModel(this UserDto userDto)
        => new User
        {
            Id = userDto.Id,
            Name = userDto.Name,
            CreatedAt = userDto.CreatedAt,
        };
}