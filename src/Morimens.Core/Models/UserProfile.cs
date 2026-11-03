namespace Morimens.Core.Models;

/// <summary>
/// Extended profile data for a registered user.
/// Keyed by the ASP.NET Identity user ID (string GUID).
/// </summary>
public class UserProfile
{
    public string  UserId      { get; set; } = "";
    public string  DisplayName { get; set; } = "";
    public string? AvatarUrl   { get; set; }
    public DateTime CreatedAt  { get; set; } = DateTime.UtcNow;

    // Navigation (EF populated; null-safe in application code)
    public UserSettings?           Settings    { get; set; }
    public List<SavedSimulation>   Simulations { get; set; } = [];
}
