using Morimens.Core.Models;

namespace Morimens.Core.Pipeline;

public class DamageContext
{
    public required Awakener          Attacker         { get; set; }
    public required EnemyInfo         Target           { get; set; }
    public required List<SkillEffect> ActiveEffects    { get; set; }

    /// <summary>ATK% coefficient for this hit (from the active skill).</summary>
    public decimal AtkPercentScaling  { get; set; }

    /// <summary>Flat base printed on the card; applied as a multiplier at Layer 1.</summary>
    public decimal? CardBaseDamage    { get; set; }

    /// <summary>Skill type label for SpecificBaseDmgPercent gating (e.g. "Strike").</summary>
    public string?  SkillTypeTag      { get; set; }

    public bool    IsCritical          { get; set; }
    public decimal BaseCritMultiplier  { get; set; } = 1.5m;

    // Mutable working state
    public decimal CurrentDamage      { get; set; }

    // Audit trail; Layer 5 result snapshot = displayed card damage
    public List<PipelineStep> Steps        { get; set; } = [];
    public decimal DisplayedDamage         { get; set; }  // snapshot after Layer 5
    public decimal CritMultiplierApplied   { get; set; }  // set by Layer 6 if crit
}
