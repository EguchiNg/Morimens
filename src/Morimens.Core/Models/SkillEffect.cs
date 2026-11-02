using Morimens.Core.Enums;

namespace Morimens.Core.Models;

public class SkillEffect
{
    public int        Id           { get; set; }
    public EffectType Type         { get; set; }
    public decimal    Value        { get; set; }
    /// <summary>
    /// For SpecificBaseDmgPercent: the skill type this effect applies to (e.g. "Strike").
    /// Null means the effect is unconditional.
    /// </summary>
    public string?    ConditionTag { get; set; }
}
