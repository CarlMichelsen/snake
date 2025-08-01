using Domain;
using Domain.Snake;

namespace Presentation.Auth;

public class Session(User user)
{
    public Guid SessionId { get; } = Guid.CreateVersion7();

    public Lobby? Lobby { get; set; }
    
    public User User => user;
}