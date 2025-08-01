using Presentation.Auth.Dto;

namespace Presentation.Auth;

public interface ISimpleLogin
{
    Session? Login(LoginDto login);
    
    /// <summary>
    /// Returns true if a user was logged out.
    /// </summary>
    /// <returns>True if a logout was successful - false if logout failed or user not logged in to begin with.</returns>
    bool Logout();

    Session? GetSession();
}