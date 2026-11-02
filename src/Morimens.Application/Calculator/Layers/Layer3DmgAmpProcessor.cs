using Morimens.Core.Enums;
using Morimens.Core.Interfaces;
using Morimens.Core.Pipeline;

namespace Morimens.Application.Calculator.Layers;

/// <summary>
/// Layer 3 — DMG AMPLIFICATION
/// All sources pool additively, then ceiling is taken.
/// Does NOT apply to flat additions added in Layer 4.
/// </summary>
public class Layer3DmgAmpProcessor : IDamageLayer
{
    public string Name  => "Layer3_DmgAmp";
    public int    Order => 3;

    public void Apply(DamageContext ctx)
    {
        decimal before = ctx.CurrentDamage;
        decimal pool   = ctx.ActiveEffects
            .Where(e => e.Type == EffectType.DmgAmplification)
            .Sum(e => e.Value);

        ctx.CurrentDamage = Math.Ceiling(before * (1 + pool));

        ctx.Steps.Add(new PipelineStep(
            Name, before, ctx.CurrentDamage,
            pool != 0
                ? $"⌈× (1 + {pool:F3} pooled amp)⌉"
                : "No DMG Amplification"
        ));
    }
}
