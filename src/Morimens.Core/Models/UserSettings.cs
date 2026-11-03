namespace Morimens.Core.Models;

/// <summary>
/// Per-user UI and calculator preferences.
/// One-to-one with UserProfile.
/// </summary>
public class UserSettings
{
    public string  UserId               { get; set; } = "";

    // UI
    public string  Theme                { get; set; } = "dark";   // "dark" | "light"

    // Calculator defaults
    public int?    DefaultAwakenerId    { get; set; }
    public decimal DefaultCritMultiplier { get; set; } = 1.5m;
    public bool    DefaultIsCritical    { get; set; } = false;
}
