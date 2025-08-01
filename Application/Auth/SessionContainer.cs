using Domain;
using Presentation.Auth;

namespace Application.Auth;

public class SessionContainer : ISessionContainer
{
    private readonly Dictionary<Guid, Session> sessions = [];

    public IReadOnlyDictionary<Guid, Session> Sessions => this.sessions;
    
    public Session? AddSession(Session session)
    {
        if (!this.ValidateUser(session.User))
        {
            return null;
        }
        
        return this.sessions.TryAdd(session.SessionId, session)
            ? session
            : null;
    }

    public bool InvalidateSession(Guid sessionId)
    {
        return this.sessions.Remove(sessionId);
    }
    
    private static string Capitalize(ReadOnlySpan<char> word)
    {
        if (word.IsEmpty)
        {
            return string.Empty;
        }

        Span<char> result = stackalloc char[word.Length];
        word.CopyTo(result);
        result[0] = char.ToUpper(result[0]);
        return new string(result);
    }

    private bool ValidateUser(User user)
    {
        user.Name = Capitalize(user.Name);
        var existingUser = this.Sessions.Values.FirstOrDefault(s =>
            s.User.Name.Equals(user.Name, StringComparison.InvariantCultureIgnoreCase));
        return existingUser is null;
    }
}