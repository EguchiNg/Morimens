using System.ComponentModel.DataAnnotations;

namespace Morimens.Api.DTOs.Requests;

public class UpdateProfileRequest
{
    [MaxLength(64)]
    public string? DisplayName { get; set; }

    [MaxLength(512)]
    public string? AvatarUrl   { get; set; }
}

public class UpdateSettingsRequest
{
    [RegularExpression("^(dark|light)$")]
    public string? Theme                 { get; set; }
    public int?    DefaultAwakenerId     { get; set; }
    public decimal? DefaultCritMultiplier { get; set; }
    public bool?   DefaultIsCritical     { get; set; }
}

public class SaveSimulationRequest
{
    [Required, MaxLength(128)]
    public string Name       { get; set; } = "";

    [Required]
    public object InputsJson { get; set; } = new();   // raw JSON object from client

    [Required]
    public object ResultJson { get; set; } = new();
}
