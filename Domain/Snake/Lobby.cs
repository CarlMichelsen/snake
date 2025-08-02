using System.Collections.Concurrent;

namespace Domain.Snake;

public class Lobby(UserConnection creatorConnection, TimeProvider timeProvider)
{
    public Guid Id { get; } = Guid.CreateVersion7();
    
    public Guid LobbyLeaderId { get; set; } = creatorConnection.User.Id;

    public List<ChatMessage> Messages { get; } = [];
    
    public ConcurrentDictionary<Guid, UserConnection> Users { get; } =
        new([new KeyValuePair<Guid, UserConnection>(creatorConnection.User.Id, creatorConnection)]);

    public void Join(User user)
    {
        if (!this.Users.TryAdd(user.Id, new UserConnection { Active = true, User = user }))
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
        if (!this.Users.TryGetValue(senderUserId, out var userConnection))
        {
            throw new SnakeException("User was not found when sending message");
        }

        var message = new ChatMessage(
            Id: Guid.CreateVersion7(),
            Sender: userConnection.User,
            Content: content,
            TimeStamp: timeProvider.GetUtcNow());
        
        this.Messages.Add(message);
        return message;
    }
}