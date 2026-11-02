namespace Morimens.Core.Models;

public class Skill
{
    public int     Id                { get; set; }
    public string  Name              { get; set; } = "";
    public string  Description       { get; set; } = "";

    /// <summary>ATK% coefficient seeding Layer 1. E.g. 1.5 = "150% ATK".</summary>
    public decimal AtkPercentScaling { get; set; }

    /// <summary>
    /// Flat base printed on the card (non-multi/each-copy cards only).
    /// When set, this multiplies the Layer 1 seed.
    /// </summary>
    public decimal? CardBaseDamage   { get; set; }

    /// <summary>
    /// Type label used for SpecificBaseDmgPercent gating. E.g. "Strike", "Command".
    /// </summary>
    public string?  SkillTypeTag     { get; set; }

    public List<SkillEffect> Effects { get; set; } = [];
}
