using Microsoft.AspNetCore.Mvc;
using Morimens.Api.DTOs.Requests;
using Morimens.Api.DTOs.Responses;
using Morimens.Core.Interfaces;
using Morimens.Core.Models;
using Morimens.Core.Pipeline;

namespace Morimens.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DamageController : ControllerBase
{
    private readonly IAwakenerRepository _awakenerRepo;
    private readonly IDamageCalculator   _calculator;

    public DamageController(IAwakenerRepository awakenerRepo, IDamageCalculator calculator)
    {
        _awakenerRepo = awakenerRepo;
        _calculator   = calculator;
    }

    /// <summary>
    /// Calculates damage for one hit: attacker + skill + target + optional extra effects.
    /// Returns the displayed value (Layer 5) and actual value (Layer 6) with a full breakdown.
    /// </summary>
    [HttpPost("calculate")]
    public ActionResult<DamageCalculationResponse> Calculate([FromBody] DamageCalculationRequest req)
    {
        // Resolve attacker
        var awakener = _awakenerRepo.GetById(req.AwakenerId);
        if (awakener is null)
            return NotFound($"Awakener {req.AwakenerId} not found.");

        // Resolve skill
        var skill = awakener.Skills.FirstOrDefault(s => s.Id == req.SkillId);
        if (skill is null)
            return NotFound($"Skill {req.SkillId} not found on Awakener {req.AwakenerId}.");

        // Build active effects: skill effects + any caller-supplied extras
        var effects = new List<SkillEffect>(skill.Effects);
        foreach (var e in req.ExtraEffects)
        {
            effects.Add(new SkillEffect
            {
                Type         = e.Type,
                Value        = e.Value,
                ConditionTag = e.ConditionTag
            });
        }

        // Build context
        var ctx = new DamageContext
        {
            Attacker           = awakener,
            Target             = new EnemyInfo
            {
                Name      = req.Target.Name,
                Def       = req.Target.Def,
                ShieldHp  = req.Target.ShieldHp,
                Tags      = req.Target.Tags,
                DefFactor = req.Target.DefFactor
            },
            ActiveEffects      = effects,
            AtkPercentScaling  = skill.AtkPercentScaling,
            CardBaseDamage     = skill.CardBaseDamage,
            SkillTypeTag       = skill.SkillTypeTag,
            IsCritical         = req.IsCritical,
            BaseCritMultiplier = req.BaseCritMultiplier,
        };

        var result = _calculator.Calculate(ctx);

        return Ok(new DamageCalculationResponse
        {
            DisplayedDamage       = result.DisplayedDamage,
            FinalDamage           = result.FinalDamage,
            IsCritical            = result.IsCritical,
            CritMultiplierApplied = result.CritMultiplierApplied,
            ShieldDamage          = result.ShieldDamage,
            OverflowDamage        = result.OverflowDamage,
            HpDamage              = result.HpDamage,
            ShieldBroken          = result.ShieldBroken,
            Breakdown             = result.Breakdown.Select(s => new PipelineStepResponse
            {
                LayerName    = s.LayerName,
                DamageBefore = s.DamageBefore,
                DamageAfter  = s.DamageAfter,
                Description  = s.Description
            }).ToList()
        });
    }
}
