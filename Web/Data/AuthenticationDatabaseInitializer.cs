using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Web.Authentication;

namespace Web.Data;

public sealed class AuthenticationDatabaseInitializer(
    AuthenticationDbContext database,
    IConfiguration configuration,
    IPasswordHasher<AuthenticationUser> passwordHasher)
{
    public async Task InitializeAsync(bool seedDevelopmentUser)
    {
        await database.Database.EnsureCreatedAsync();

        if (!seedDevelopmentUser || await database.Users.AnyAsync())
        {
            return;
        }

        var firstName = configuration["Authentication:DevelopmentSeedUser:FirstName"] ?? "Jordan";
        var lastName = configuration["Authentication:DevelopmentSeedUser:LastName"] ?? "Example";
        var password = configuration["Authentication:DevelopmentSeedUser:Password"]
            ?? throw new InvalidOperationException("The development seed password is required.");
        var user = new AuthenticationUser
        {
            FirstName = firstName,
            LastName = lastName,
            Username = await GenerateUsernameAsync(firstName, lastName),
            PasswordHash = string.Empty
        };
        user.PasswordHash = passwordHasher.HashPassword(user, password);

        database.Users.Add(user);
        await database.SaveChangesAsync();
    }

    private async Task<string> GenerateUsernameAsync(string firstName, string lastName)
    {
        var normalizedFirstName = NormalizeName(firstName);
        var normalizedLastName = NormalizeName(lastName);

        if (normalizedFirstName.Length == 0 || normalizedLastName.Length == 0)
        {
            throw new ArgumentException("First and last names must contain at least one letter.");
        }

        var prefix = normalizedLastName[..Math.Min(6, normalizedLastName.Length)] + normalizedFirstName[0];
        var usernames = await database.Users
            .Where(candidate => candidate.Username.StartsWith(prefix))
            .Select(candidate => candidate.Username)
            .ToListAsync();

        if (!usernames.Contains(prefix, StringComparer.OrdinalIgnoreCase))
        {
            return prefix;
        }

        var suffix = 2;
        while (usernames.Contains($"{prefix}{suffix}", StringComparer.OrdinalIgnoreCase))
        {
            suffix++;
        }

        return $"{prefix}{suffix}";
    }

    private static string NormalizeName(string value) => new string(value.Where(char.IsLetter).ToArray()).ToLowerInvariant();
}
