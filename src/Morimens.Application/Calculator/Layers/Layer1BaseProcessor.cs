using Morimens.Core.Interfaces;
using Morimens.Core.Pipeline;

namespace Morimens.Application.Calculator.Layers;

/// <summary>
/// Layer 1 — BASE
/// Seeds from ATK × AtkPercentScaling (ceiling), then multiplies by
/// the card's printed base damage if present.
/// </summary>
public class Layer1BaseProcessor : IDamageLayer
{
    public string Name  => "Layer1_Base";
    public int    Order => 1;

    public void Apply(DamageContext ctx)
    {
        decimal value = Math.Ceiling(ctx.Attacker.BaseAtk * ctx.AtkPercentScaling);

        if (ctx.CardBaseDamage.HasValue)
            value *= ctx.CardBaseDamage.Value;

        ctx.CurrentDamage = value;
        ctx.Steps.Add(new PipelineStep(
            Name,
            0m,
            value,
            $"⌈{ctx.Attacker.BaseAtk} ATK × {ctx.AtkPercentScaling}⌉"
            + (ctx.CardBaseDamage.HasValue ? $" × {ctx.CardBaseDamage} card base" : "")
        ));
    }
}
