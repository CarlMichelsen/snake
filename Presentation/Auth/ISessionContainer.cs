namespace Presentation.Auth;

public interface ISessionContainer
{
    IReadOnlyDictionary<Guid, Session> Sessions { get; }
    
    Session? AddSession(Session session);
    
    bool InvalidateSession(Guid sessionId);
}