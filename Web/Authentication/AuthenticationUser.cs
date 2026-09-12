using System.Security.Claims;

namespace Web.Authentication;

public sealed class AuthenticationUser
{
    public int Id { get; set; }

    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public required string Username { get; set; }

    public required string PasswordHash { get; set; }

    public static ClaimsPrincipal CreatePrincipal(AuthenticationUser user)
    {
        var identity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username)
        ], "Cookies");

        return new ClaimsPrincipal(identity);
    }
}
