using Morimens.Core.Pipeline;

namespace Morimens.Core.Results;

public class DamageResult
{
    public decimal DisplayedDamage      { get; set; }
    public decimal FinalDamage          { get; set; }
    public bool    IsCritical           { get; set; }
    public decimal CritMultiplierApplied { get; set; }

    public decimal? ShieldDamage        { get; set; }
    public decimal? OverflowDamage      { get; set; }
    public decimal? HpDamage            { get; set; }
    public bool     ShieldBroken        { get; set; }

    public List<PipelineStep> Breakdown { get; set; } = [];
}
