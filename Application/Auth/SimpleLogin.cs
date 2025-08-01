using Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Presentation.Auth;
using Presentation.Auth.Dto;

namespace Application.Auth;

/// <summary>
/// This is insanely unsafe - but it will work for now.
/// </summary>
/// <param name="httpContextAccessor">Read/Write from http context.</param>
public class SimpleLogin(
    TimeProvider timeProvider,
    ILogger<SimpleLogin> logger,
    ISessionContainer sessionContainer,
    IWebHostEnvironment webHostEnvironment,
    IHttpContextAccessor httpContextAccessor) : ISimpleLogin
{
    private const string CookieName = "auth";
    
    private readonly CookieOptions options = new()
    {
        Path = "/",
        HttpOnly = true,
        Secure = webHostEnvironment.IsProduction(), // Set to true in production with HTTPS
        SameSite = webHostEnvironment.IsProduction() ? SameSiteMode.Strict : SameSiteMode.Lax,
        MaxAge = TimeSpan.FromDays(60),
    };
    
    public Session? Login(LoginDto login)
    {
        var existingSession = this.GetSession();
        if (existingSession is not null)
        {
            logger.LogWarning(
                "User '{Username}'<{Identifier}> attempted to log in with new user '{NewUsername}' - original session returned instead",
                existingSession.User.Name,
                existingSession.User.Id,
                login.Username);
            return sessionContainer.Sessions.GetValueOrDefault(existingSession.SessionId);
        }
        
        var session = sessionContainer.AddSession(new Session(new User
        {
            Id = Guid.CreateVersion7(),
            Name = login.Username, 
            CreatedAt = timeProvider.GetUtcNow(),
        }));

        if (session is null)
        {
            return null;
        }
        
        httpContextAccessor
            .HttpContext?
            .Response
            .Cookies
            .Append(CookieName, session.SessionId.ToString(), this.options);
        
        logger.LogInformation(
            "User '{Username}'<{Identifier}> was created",
            session.User.Name,
            session.User.Id);

        return session;
    }

    public bool Logout()
    {
        var session = this.GetSession();
        if (session is not null && sessionContainer.InvalidateSession(session.SessionId))
        {
            httpContextAccessor
                .HttpContext?
                .Response
                .Cookies
                .Delete(CookieName, this.options);
            
            logger.LogInformation(
                "User '{Username}'<{Identifier}> was deleted (logged out)",
                session.User.Name,
                session.User.Id);
            
            return true;
        }

        return false;
    }

    public Session? GetSession()
    {
        var cookies = httpContextAccessor.HttpContext?.Request.Cookies;
        if (cookies?.TryGetValue(CookieName, out var value) != true)
        {
            return null;
        }

        if (Guid.TryParse(value, out var userId) &&
            sessionContainer.Sessions.TryGetValue(userId, out var user))
        {
            return user;
        }
        
        return null;
    }
}