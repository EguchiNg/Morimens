using Microsoft.AspNetCore.Identity;

namespace Morimens.Infrastructure.Identity;

/// <summary>
/// Extends IdentityUser with optional Discord link.
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>Discord user ID, set when the account was created or linked via Discord OAuth.</summary>
    public string? DiscordId { get; set; }
}
