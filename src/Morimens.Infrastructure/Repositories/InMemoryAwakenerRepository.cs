using Morimens.Core.Enums;
using Morimens.Core.Interfaces;
using Morimens.Core.Models;

namespace Morimens.Infrastructure.Repositories;

/// <summary>
/// In-memory seed data for MVP; replace with a DB-backed repository later.
/// </summary>
public class InMemoryAwakenerRepository : IAwakenerRepository
{
    private readonly IReadOnlyList<Awakener> _data;

    public InMemoryAwakenerRepository()
    {
        _data = new List<Awakener>
        {
            new Awakener
            {
                Id      = 1,
                Name    = "Placeholder A",
                BaseAtk = 1200,
                BaseDef = 600,
                BaseHp  = 8000,
                Tags    = ["Strike"],
                Skills  =
                [
                    new Skill
                    {
                        Id                = 101,
                        Name              = "Basic Strike",
                        Description       = "A standard physical attack.",
                        AtkPercentScaling = 1.0m,
                        CardBaseDamage    = null,
                        SkillTypeTag      = "Strike",
                        Effects           = []
                    },
                    new Skill
                    {
                        Id                = 102,
                        Name              = "Heavy Strike",
                        Description       = "A hard-hitting strike with bonus Base DMG.",
                        AtkPercentScaling = 1.5m,
                        CardBaseDamage    = 1.2m,
                        SkillTypeTag      = "Strike",
                        Effects           =
                        [
                            new SkillEffect
                            {
                                Id           = 1001,
                                Type         = EffectType.GeneralBaseDmgPercent,
                                Value        = 0.30m,   // +30 % base dmg
                                ConditionTag = null
                            }
                        ]
                    }
                ]
            },

            new Awakener
            {
                Id      = 2,
                Name    = "Placeholder B",
                BaseAtk = 1000,
                BaseDef = 500,
                BaseHp  = 9000,
                Tags    = ["Command"],
                Skills  =
                [
                    new Skill
                    {
                        Id                = 201,
                        Name              = "Command Order",
                        Description       = "Issues a command that amplifies ally damage.",
                        AtkPercentScaling = 0.8m,
                        CardBaseDamage    = null,
                        SkillTypeTag      = "Command",
                        Effects           =
                        [
                            new SkillEffect
                            {
                                Id           = 2001,
                                Type         = EffectType.DmgAmplification,
                                Value        = 0.20m,   // +20 % amp
                                ConditionTag = null
                            }
                        ]
                    }
                ]
            },

            new Awakener
            {
                Id      = 3,
                Name    = "Placeholder C",
                BaseAtk = 1100,
                BaseDef = 550,
                BaseHp  = 8500,
                Tags    = ["Strike", "Support"],
                Skills  =
                [
                    new Skill
                    {
                        Id                = 301,
                        Name              = "Combo Finisher",
                        Description       = "Deals high final-damage-boosted damage.",
                        AtkPercentScaling = 2.0m,
                        CardBaseDamage    = 1.5m,
                        SkillTypeTag      = "Strike",
                        Effects           =
                        [
                            new SkillEffect
                            {
                                Id           = 3001,
                                Type         = EffectType.FinalDmg,
                                Value        = 0.25m,   // +25 % final dmg
                                ConditionTag = null
                            },
                            new SkillEffect
                            {
                                Id           = 3002,
                                Type         = EffectType.SpecificBaseDmgPercent,
                                Value        = 0.40m,   // +40 % only for Strike skills
                                ConditionTag = "Strike"
                            }
                        ]
                    }
                ]
            }
        }.AsReadOnly();
    }

    public IReadOnlyList<Awakener> GetAll()                  => _data;
    public Awakener?               GetById(int id)           => _data.FirstOrDefault(a => a.Id == id);
    public IReadOnlyList<Awakener> Search(string name)       =>
        _data.Where(a => a.Name.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
}
