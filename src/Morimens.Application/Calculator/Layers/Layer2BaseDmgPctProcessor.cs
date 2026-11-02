using Morimens.Core.Enums;
using Morimens.Core.Interfaces;
using Morimens.Core.Pipeline;

namespace Morimens.Application.Calculator.Layers;

/// <summary>
/// Layer 2 — BASE DMG %
/// Each source multiplies independently (one circle per source).
/// SpecificBaseDmgPercent only fires when ConditionTag matches the active SkillTypeTag.
/// </summary>
public class Layer2BaseDmgPctProcessor : IDamageLayer
{
    public string Name  => "Layer2_BaseDmgPct";
    public int    Order => 2;

    public void Apply(DamageContext ctx)
    {
        decimal before = ctx.CurrentDamage;
        decimal value  = before;
        int     count  = 0;

        foreach (var fx in ctx.ActiveEffects.Where(e =>
            e.Type is EffectType.GeneralBaseDmgPercent or EffectType.SpecificBaseDmgPercent))
        {
            if (fx.Type == EffectType.SpecificBaseDmgPercent &&
                !string.Equals(fx.ConditionTag, ctx.SkillTypeTag, StringComparison.OrdinalIgnoreCase))
                continue;

            value *= (1 + fx.Value);
            count++;
        }

        ctx.CurrentDamage = value;
        ctx.Steps.Add(new PipelineStep(
            Name, before, value,
            count > 0 ? $"× Base DMG% ({count} source{(count == 1 ? "" : "s")})" : "No Base DMG% bonuses"
        ));
    }
}
