using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace Morimens.WPF.Services;

// ── DTOs (mirrors Morimens.Api DTOs) ─────────────────────────────────────────

public class AwakenerDto
{
    public int    Id      { get; set; }
    public string Name    { get; set; } = "";
    public int    BaseAtk { get; set; }
    public int    BaseDef { get; set; }
    public int    BaseHp  { get; set; }
    public List<string>   Tags   { get; set; } = [];
    public List<SkillDto> Skills { get; set; } = [];
}

public class SkillDto
{
    public int     Id                { get; set; }
    public string  Name              { get; set; } = "";
    public string  Description       { get; set; } = "";
    public decimal AtkPercentScaling { get; set; }
    public decimal? CardBaseDamage   { get; set; }
    public string?  SkillTypeTag     { get; set; }
}

public class DamageCalculationRequestDto
{
    public int     AwakenerId         { get; set; }
    public int     SkillId            { get; set; }
    public bool    IsCritical         { get; set; }
    public decimal BaseCritMultiplier { get; set; } = 1.5m;
    public TargetDto Target           { get; set; } = new();
    public List<ExtraEffectDto> ExtraEffects { get; set; } = [];
}

public class TargetDto
{
    public int     Def       { get; set; }
    public int?    ShieldHp  { get; set; }
    public decimal DefFactor { get; set; } = 1.0m;
}

public class ExtraEffectDto
{
    public string  Type         { get; set; } = "";
    public decimal Value        { get; set; }
    public string? ConditionTag { get; set; }
}

public class DamageResultDto
{
    public decimal DisplayedDamage       { get; set; }
    public decimal FinalDamage           { get; set; }
    public bool    IsCritical            { get; set; }
    public decimal CritMultiplierApplied { get; set; }
    public decimal? ShieldDamage         { get; set; }
    public decimal? OverflowDamage       { get; set; }
    public decimal? HpDamage            { get; set; }
    public bool     ShieldBroken         { get; set; }
    public List<PipelineStepDto> Breakdown { get; set; } = [];
}

public class PipelineStepDto
{
    public string  LayerName    { get; set; } = "";
    public decimal DamageBefore { get; set; }
    public decimal DamageAfter  { get; set; }
    public string  Description  { get; set; } = "";
}

// ── Client ────────────────────────────────────────────────────────────────────

public class ApiClient
{
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _json = new()
    {
        PropertyNameCaseInsensitive = true
    };

    // Change this if your API runs on a different port
    public ApiClient(string baseUrl = "http://localhost:5000")
    {
        _http = new HttpClient { BaseAddress = new Uri(baseUrl) };
    }

    public async Task<List<AwakenerDto>> GetAwakeners()
    {
        var result = await _http.GetFromJsonAsync<List<AwakenerDto>>("api/awakener", _json);
        return result ?? [];
    }

    public async Task<DamageResultDto> Calculate(DamageCalculationRequestDto request)
    {
        var response = await _http.PostAsJsonAsync("api/damage/calculate", request);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<DamageResultDto>(_json);
        return result!;
    }
}
