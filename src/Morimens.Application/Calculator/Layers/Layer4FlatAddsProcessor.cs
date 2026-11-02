using Morimens.Core.Enums;
using Morimens.Core.Interfaces;
using Morimens.Core.Pipeline;

namespace Morimens.Application.Calculator.Layers;

/// <summary>
/// Layer 4 — STR AND FLAT ADDS
/// Flat damage additions are summed and added to the amplified base.
/// These are NOT multiplied by DMG Amplification from Layer 3.
/// </summary>
public class Layer4FlatAddsProcessor : IDamageLayer
{
    public string Name  => "Layer4_FlatAdds";
    public int    Order => 4;

    public void Apply(DamageContext ctx)
    {
        decimal before = ctx.CurrentDamage;
        decimal flat   = ctx.ActiveEffects
            .Where(e => e.Type == EffectType.StrFlatAdd)
            .Sum(e => e.Value);

        ctx.CurrentDamage = before + flat;

        ctx.Steps.Add(new PipelineStep(
            Name, before, ctx.CurrentDamage,
            flat != 0
                ? $"+ {flat} flat adds"
                : "No flat additions"
        ));
    }
}
