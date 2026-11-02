namespace Morimens.Api.DTOs.Responses;

public class AwakenerResponse
{
    public int    Id      { get; set; }
    public string Name    { get; set; } = "";
    public int    BaseAtk { get; set; }
    public int    BaseDef { get; set; }
    public int    BaseHp  { get; set; }
    public List<string>       Tags   { get; set; } = [];
    public List<SkillResponse> Skills { get; set; } = [];
}

public class SkillResponse
{
    public int     Id                { get; set; }
    public string  Name              { get; set; } = "";
    public string  Description       { get; set; } = "";
    public decimal AtkPercentScaling { get; set; }
    public decimal? CardBaseDamage   { get; set; }
    public string? SkillTypeTag      { get; set; }
    public List<SkillEffectResponse> Effects { get; set; } = [];
}

public class SkillEffectResponse
{
    public int    Id           { get; set; }
    public string Type         { get; set; } = "";
    public decimal Value       { get; set; }
    public string? ConditionTag { get; set; }
}
