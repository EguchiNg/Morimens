namespace Morimens.Core.Models;

/// <summary>Runtime enemy descriptor passed into a damage calculation.</summary>
public class EnemyInfo
{
    public string       Name     { get; set; } = "Enemy";
    public int          Def      { get; set; }
    public int?         ShieldHp { get; set; }
    public List<string> Tags     { get; set; } = [];

    /// <summary>
    /// DEF reduction factor applied in Layer 6.
    /// Placeholder value 1.0 until the real formula is confirmed from in-game data.
    /// </summary>
    public decimal DefFactor { get; set; } = 1.0m;
}
