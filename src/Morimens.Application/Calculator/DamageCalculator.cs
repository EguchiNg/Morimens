using Morimens.Application.Calculator.Layers;
using Morimens.Core.Interfaces;
using Morimens.Core.Models;
using Morimens.Core.Pipeline;
using Morimens.Core.Results;

namespace Morimens.Application.Calculator;

/// <summary>
/// Runs the 6-layer damage pipeline and produces a <see cref="DamageResult"/>.
/// </summary>
public class DamageCalculator : IDamageCalculator
{
    private readonly IReadOnlyList<IDamageLayer> _layers;

    public DamageCalculator()
    {
        _layers = new List<IDamageLayer>
        {
            new Layer1BaseProcessor(),
            new Layer2BaseDmgPctProcessor(),
            new Layer3DmgAmpProcessor(),
            new Layer4FlatAddsProcessor(),
            new Layer5FinalDmgProcessor(),
            new Layer6TargetCritProcessor(),
        }
        .OrderBy(l => l.Order)
        .ToList()
        .AsReadOnly();
    }

    public DamageResult Calculate(DamageContext ctx)
    {
        foreach (var layer in _layers)
            layer.Apply(ctx);

        decimal actual = ctx.CurrentDamage;

        // Shield / overflow split
        decimal? shieldDmg    = null;
        decimal? overflowDmg  = null;
        decimal? hpDmg        = null;
        bool     shieldBroken = false;

        if (ctx.Target.ShieldHp.HasValue && ctx.Target.ShieldHp.Value > 0)
        {
            shieldDmg = Math.Min(actual, ctx.Target.ShieldHp.Value);
            if (actual >= ctx.Target.ShieldHp.Value)
            {
                shieldBroken = true;
                overflowDmg  = actual - ctx.Target.ShieldHp.Value;
                hpDmg        = overflowDmg;
            }
        }
        else
        {
            hpDmg = actual;
        }

        return new DamageResult
        {
            DisplayedDamage       = ctx.DisplayedDamage,
            FinalDamage           = actual,
            IsCritical            = ctx.IsCritical,
            CritMultiplierApplied = ctx.CritMultiplierApplied,
            ShieldDamage          = shieldDmg,
            OverflowDamage        = overflowDmg,
            HpDamage              = hpDmg,
            ShieldBroken          = shieldBroken,
            Breakdown             = ctx.Steps,
        };
    }
}
