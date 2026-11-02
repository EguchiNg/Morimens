namespace Morimens.Core.Pipeline;

public record PipelineStep(
    string  LayerName,
    decimal DamageBefore,
    decimal DamageAfter,
    string  Description
);
