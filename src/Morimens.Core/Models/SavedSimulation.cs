namespace Morimens.Core.Models;

/// <summary>
/// A named snapshot of a damage calculation — inputs + full result — saved to a user's profile.
/// </summary>
public class SavedSimulation
{
    public int      Id          { get; set; }
    public string   UserId      { get; set; } = "";
    public string   Name        { get; set; } = "Untitled";
    public DateTime CreatedAt   { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt   { get; set; } = DateTime.UtcNow;

    /// <summary>JSON-serialised DamageCalculationRequest.</summary>
    public string   InputsJson  { get; set; } = "{}";

    /// <summary>JSON-serialised DamageResult (the full breakdown).</summary>
    public string   ResultJson  { get; set; } = "{}";
}
