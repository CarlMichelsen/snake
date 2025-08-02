using Domain;

namespace Domain.Snake;

public class UserConnection
{
    public required User User { get; init; }

    public required bool Active { get; set; }
}