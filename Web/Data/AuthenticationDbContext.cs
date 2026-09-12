using Microsoft.EntityFrameworkCore;
using Web.Authentication;

namespace Web.Data;

public sealed class AuthenticationDbContext(DbContextOptions<AuthenticationDbContext> options) : DbContext(options)
{
    public DbSet<AuthenticationUser> Users => Set<AuthenticationUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var user = modelBuilder.Entity<AuthenticationUser>();
        user.HasIndex(candidate => candidate.Username).IsUnique();
        user.Property(candidate => candidate.FirstName).HasMaxLength(100);
        user.Property(candidate => candidate.LastName).HasMaxLength(100);
        user.Property(candidate => candidate.Username).HasMaxLength(100);
    }
}
