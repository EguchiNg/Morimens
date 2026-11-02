using Microsoft.AspNetCore.Mvc;
using Morimens.Api.DTOs.Responses;
using Morimens.Core.Interfaces;
using Morimens.Core.Models;

namespace Morimens.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AwakenerController : ControllerBase
{
    private readonly IAwakenerRepository _repo;

    public AwakenerController(IAwakenerRepository repo)
    {
        _repo = repo;
    }

    /// <summary>Returns all Awakeners (id, name, base stats, skills).</summary>
    [HttpGet]
    public ActionResult<IEnumerable<AwakenerResponse>> GetAll()
    {
        return Ok(_repo.GetAll().Select(Map));
    }

    /// <summary>Returns a single Awakener by ID.</summary>
    [HttpGet("{id:int}")]
    public ActionResult<AwakenerResponse> GetById(int id)
    {
        var awakener = _repo.GetById(id);
        if (awakener is null) return NotFound();
        return Ok(Map(awakener));
    }

    /// <summary>Searches Awakeners by name substring.</summary>
    [HttpGet("search")]
    public ActionResult<IEnumerable<AwakenerResponse>> Search([FromQuery] string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return BadRequest("Query parameter 'name' is required.");

        return Ok(_repo.Search(name).Select(Map));
    }

    // ── Mapping ──────────────────────────────────────────────────────────────

    private static AwakenerResponse Map(Awakener a) => new()
    {
        Id      = a.Id,
        Name    = a.Name,
        BaseAtk = a.BaseAtk,
        BaseDef = a.BaseDef,
        BaseHp  = a.BaseHp,
        Tags    = a.Tags,
        Skills  = a.Skills.Select(s => new SkillResponse
        {
            Id                = s.Id,
            Name              = s.Name,
            Description       = s.Description,
            AtkPercentScaling = s.AtkPercentScaling,
            CardBaseDamage    = s.CardBaseDamage,
            SkillTypeTag      = s.SkillTypeTag,
            Effects           = s.Effects.Select(e => new SkillEffectResponse
            {
                Id           = e.Id,
                Type         = e.Type.ToString(),
                Value        = e.Value,
                ConditionTag = e.ConditionTag
            }).ToList()
        }).ToList()
    };
}
