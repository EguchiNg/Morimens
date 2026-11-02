namespace Morimens.Api.DTOs.Responses;

public class DamageCalculationResponse
{
    /// <summary>Damage shown on the card (after Layer 5, before target/crit).</summary>
    public decimal DisplayedDamage        { get; set; }

    /// <summary>Actual damage dealt (after all 6 layers).</summary>
    public decimal FinalDamage            { get; set; }

    public bool    IsCritical             { get; set; }
    public decimal CritMultiplierApplied  { get; set; }

    public decimal? ShieldDamage          { get; set; }
    public decimal? OverflowDamage        { get; set; }
    public decimal? HpDamage             { get; set; }
    public bool     ShieldBroken          { get; set; }

    /// <summary>Step-by-step layer breakdown for the UI.</summary>
    public List<PipelineStepResponse> Breakdown { get; set; } = [];
}

public class PipelineStepResponse
{
    public string  LayerName     { get; set; } = "";
    public decimal DamageBefore  { get; set; }
    public decimal DamageAfter   { get; set; }
    public string  Description   { get; set; } = "";
}
