using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Morimens.Core.Models;
using Morimens.Infrastructure.Identity;

namespace Morimens.Infrastructure.Data;

public class MorimensDbContext : IdentityDbContext<ApplicationUser>
{
    public MorimensDbContext(DbContextOptions<MorimensDbContext> options) : base(options) { }

    public DbSet<UserProfile>      UserProfiles      => Set<UserProfile>();
    public DbSet<UserSettings>     UserSettings      => Set<UserSettings>();
    public DbSet<SavedSimulation>  SavedSimulations  => Set<SavedSimulation>();
    public DbSet<RefreshToken>     RefreshTokens     => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // UserProfile — 1:1 with IdentityUser
        builder.Entity<UserProfile>(e =>
        {
            e.HasKey(p => p.UserId);
            e.Property(p => p.DisplayName).HasMaxLength(64).IsRequired();
            e.Property(p => p.AvatarUrl).HasMaxLength(512);
            e.HasOne(p => p.Settings)
             .WithOne()
             .HasForeignKey<UserSettings>(s => s.UserId);
        });

        // UserSettings — 1:1 with UserProfile
        builder.Entity<UserSettings>(e =>
        {
            e.HasKey(s => s.UserId);
            e.Property(s => s.Theme).HasMaxLength(16).HasDefaultValue("dark");
            e.Property(s => s.DefaultCritMultiplier).HasPrecision(5, 3);
        });

        // SavedSimulation
        builder.Entity<SavedSimulation>(e =>
        {
            e.HasKey(s => s.Id);
            e.Property(s => s.Name).HasMaxLength(128).IsRequired();
            e.Property(s => s.InputsJson).IsRequired();
            e.Property(s => s.ResultJson).IsRequired();
            e.HasIndex(s => s.UserId);
        });

        // RefreshToken
        builder.Entity<RefreshToken>(e =>
        {
            e.HasKey(r => r.Id);
            e.Property(r => r.Token).HasMaxLength(256).IsRequired();
            e.HasIndex(r => r.Token).IsUnique();
            e.HasIndex(r => r.UserId);
        });
    }
}
