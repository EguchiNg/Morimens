using Morimens.Core.Enums;
using Morimens.Core.Interfaces;
using Morimens.Core.Pipeline;

namespace Morimens.Application.Calculator.Layers;

/// <summary>
/// Layer 6 — TARGET AND CRIT
/// Each modifier multiplies independently (one circle per source).
/// Includes weakness, vulnerable, crit, DEF factor, and other target debuffs.
/// The result after this layer is the ACTUAL (real) damage dealt.
/// </summary>
public class Layer6TargetCritProcessor : IDamageLayer
{
    public string Name  => "Layer6_TargetCrit";
    public int    Order => 6;

    public void Apply(DamageContext ctx)
    {
        decimal before     = ctx.CurrentDamage;
        decimal multiplier = 1m;
        var     notes      = new List<string>();

        // Weakness multiplier
        foreach (var fx in ctx.ActiveEffects.Where(e => e.Type == EffectType.Weakness))
        {
            multiplier *= (1 + fx.Value);
            notes.Add($"×(1+{fx.Value} weakness)");
        }

        // Vulnerable multiplier
        foreach (var fx in ctx.ActiveEffects.Where(e => e.Type == EffectType.Vulnerable))
        {
            multiplier *= (1 + fx.Value);
            notes.Add($"×(1+{fx.Value} vulnerable)");
        }

        // Other target debuffs
        foreach (var fx in ctx.ActiveEffects.Where(e => e.Type == EffectType.OtherTargetDebuff))
        {
            multiplier *= (1 + fx.Value);
            notes.Add($"×(1+{fx.Value} debuff)");
        }

        // Crit
        if (ctx.IsCritical)
        {
            decimal critBonus = ctx.ActiveEffects
                .Where(e => e.Type == EffectType.CritDmgBonus)
                .Sum(e => e.Value);

            decimal critMult = ctx.BaseCritMultiplier + critBonus;
            ctx.CritMultiplierApplied = critMult;
            multiplier *= critMult;
            notes.Add($"×{critMult:F2} crit");
        }

        // DEF factor (placeholder: DefFactor already computed by the caller or default 1.0)
        if (ctx.Target.DefFactor != 1m)
        {
            multiplier *= ctx.Target.DefFactor;
            notes.Add($"×{ctx.Target.DefFactor:F3} DEF");
        }

        ctx.CurrentDamage = Math.Floor(before * multiplier);

        ctx.Steps.Add(new PipelineStep(
            Name, before, ctx.CurrentDamage,
            notes.Count > 0
                ? string.Join(" ", notes) + " → actual"
                : "No target/crit modifiers → actual"
        ));
    }

    // Convenience: not used by the pipeline runner, but available if needed
    public decimal CritMultiplierApplied { get; private set; }
}
