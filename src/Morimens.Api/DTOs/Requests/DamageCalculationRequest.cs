using Morimens.Core.Enums;

namespace Morimens.Api.DTOs.Requests;

public class DamageCalculationRequest
{
    /// <summary>ID of the attacking Awakener.</summary>
    public int AwakenerId { get; set; }

    /// <summary>ID of the skill being used (from the Awakener's skill list).</summary>
    public int SkillId { get; set; }

    /// <summary>Whether this hit is a critical strike.</summary>
    public bool IsCritical { get; set; }

    /// <summary>Base crit damage multiplier (default 1.5).</summary>
    public decimal BaseCritMultiplier { get; set; } = 1.5m;

    /// <summary>Information about the target.</summary>
    public TargetRequest Target { get; set; } = new();

    /// <summary>
    /// Additional effects active on the attacker / battlefield
    /// that are not already encoded on the skill (e.g. team buffs, field effects).
    /// </summary>
    public List<EffectRequest> ExtraEffects { get; set; } = [];
}

public class TargetRequest
{
    public string  Name      { get; set; } = "Enemy";
    public int     Def       { get; set; }
    public int?    ShieldHp  { get; set; }
    public decimal DefFactor { get; set; } = 1.0m;
    public List<string> Tags { get; set; } = [];
}

public class EffectRequest
{
    public EffectType Type         { get; set; }
    public decimal    Value        { get; set; }
    public string?    ConditionTag { get; set; }
}
