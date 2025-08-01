using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Presentation.Auth;

namespace Api.Middleware;

public class SimpleLoginMiddleware(
    ISimpleLogin simpleLogin) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var session = simpleLogin.GetSession();
        
        if (session is not null)
        {
            // Only sign in if not already authenticated with this user
            if (context.User.Identity?.IsAuthenticated != true || 
                context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value != session.SessionId.ToString())
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, session.User.Name),
                    new Claim(ClaimTypes.NameIdentifier, session.SessionId.ToString()), // SessionId
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            }
        }
        else if (context.User.Identity?.IsAuthenticated == true)
        {
            // User logged out - clear the cookie
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
        
        await next(context);
    }
}