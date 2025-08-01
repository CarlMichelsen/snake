using Application.Mappers;
using Microsoft.AspNetCore.Mvc;
using Presentation;
using Presentation.Auth;
using Presentation.Auth.Dto;

namespace Api.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var authGroup = endpoints.MapGroup("auth");
        
        authGroup
            .MapPost(
                "/login",
                ([FromServices] ISimpleLogin simpleLogin, [FromBody] LoginDto loginDto) =>
                {
                    var session = simpleLogin.Login(loginDto);
                    return session is not null
                        ? Results.Ok(new ServiceResponse<UserDto>(session.User.ToDto()))
                        : Results.Ok(new ServiceResponse("Error logging in"));
                });
        
        authGroup
            .MapDelete(
                "/logout",
                ([FromServices] ISimpleLogin simpleLogin) => simpleLogin.Logout()
                    ? Results.Ok(new ServiceResponse())
                    : Results.Ok(new ServiceResponse("Failed to log out")));
        
        authGroup
            .MapGet(
                "/user",
                ([FromServices] ISimpleLogin simpleLogin) =>
                {
                    var session = simpleLogin.GetSession();
                    return session is not null
                        ? Results.Ok(new ServiceResponse<UserDto>(session.User.ToDto()))
                        : Results.Ok(new ServiceResponse("Not logged in"));
                });
        
        return authGroup;
    }
}