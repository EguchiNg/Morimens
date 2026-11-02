using Morimens.Core.Enums;
using Morimens.Core.Interfaces;
using Morimens.Core.Pipeline;

namespace Morimens.Application.Calculator.Layers;

/// <summary>
/// Layer 5 — FINAL DMG
/// All sources pool additively, then applied as a multiplier.
/// The result after this layer is the DISPLAYED damage value shown in-game.
/// </summary>
public class Layer5FinalDmgProcessor : IDamageLayer
{
    public string Name  => "Layer5_FinalDmg";
    public int    Order => 5;

    public void Apply(DamageContext ctx)
    {
        decimal before = ctx.CurrentDamage;
        decimal pool   = ctx.ActiveEffects
            .Where(e => e.Type == EffectType.FinalDmg)
            .Sum(e => e.Value);

        ctx.CurrentDamage   = before * (1 + pool);
        ctx.DisplayedDamage = ctx.CurrentDamage;   // snapshot for UI

        ctx.Steps.Add(new PipelineStep(
            Name, before, ctx.CurrentDamage,
            pool != 0
                ? $"× (1 + {pool:F3} pooled final) → displayed"
                : "No Final DMG bonuses → displayed"
        ));
    }
}
