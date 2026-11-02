namespace Morimens.Core.Enums;

public enum EffectType
{
    // ── Layer 2 — BASE DMG % ───────────────────────────────────────────────
    GeneralBaseDmgPercent,
    SpecificBaseDmgPercent,   // conditional; gated by ConditionTag matching SkillTypeTag

    // ── Layer 3 — DMG AMPLIFICATION ───────────────────────────────────────
    DmgAmplification,         // all sources pool additively → (1 + Σ), rounds up

    // ── Layer 4 — STR AND FLAT ADDS ───────────────────────────────────────
    StrFlatAdd,               // added after Layer 3; NOT boosted by DmgAmplification

    // ── Layer 5 — FINAL DMG ───────────────────────────────────────────────
    FinalDmg,                 // all sources pool additively → (1 + Σ)

    // ── Layer 6 — TARGET AND CRIT ─────────────────────────────────────────
    Weakness,
    Vulnerable,
    CritDmgBonus,             // additive bonus to BaseCritMultiplier when IsCritical
    OtherTargetDebuff,

    // ── DEF / Shield pipeline ─────────────────────────────────────────────
    DefFlatBonus,
    DefPercentBonus,
    DefMultiplier,

    // ── Misc ──────────────────────────────────────────────────────────────
    HpFlatBonus,
    HpPercentBonus,
    IncomingDamageReduction,
}
