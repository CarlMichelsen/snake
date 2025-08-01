using System.Collections.Concurrent;

namespace Domain.Snake;

public class Lobby(User creator, TimeProvider timeProvider)
{
    public Guid Id { get; } = Guid.CreateVersion7();
    
    public User LobbyLeader { get; set; } = creator;

    public List<ChatMessage> Messages { get; } = [];
    
    public ConcurrentDictionary<Guid, User> Users { get; } = new([new KeyValuePair<Guid, User>(creator.Id, creator)]);

    public void Join(User user)
    {
        if (!this.Users.TryAdd(user.Id, user))
        {
            throw new SnakeException("User already joined");
        }
    }
    
    public void Kick(User user)
    {
        if (!this.Users.TryRemove(user.Id, out _))
        {
            throw new SnakeException("User was not found");
        }
    }

    public ChatMessage SendMessage(Guid senderUserId, string content)
    {
        if (!this.Users.TryGetValue(senderUserId, out var user))
        {
            throw new SnakeException("User was not found when sending message");
        }

        var message = new ChatMessage(
            Id: Guid.CreateVersion7(),
            Sender: user,
            Content: content,
            TimeStamp: timeProvider.GetUtcNow());
        
        this.Messages.Add(message);
        return message;
    }
}