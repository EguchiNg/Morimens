namespace Morimens.Core.Models;

public class Awakener
{
    public int          Id      { get; set; }
    public string       Name    { get; set; } = "";
    public int          BaseAtk { get; set; }
    public int          BaseDef { get; set; }
    public int          BaseHp  { get; set; }
    public List<Skill>  Skills  { get; set; } = [];
    public List<string> Tags    { get; set; } = [];
}
