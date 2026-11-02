// ── Domain types (mirror API DTOs) ───────────────────────────────────────────

export interface SkillEffect {
  id: number;
  type: string;
  value: number;
  conditionTag?: string;
}

export interface Skill {
  id: number;
  name: string;
  description: string;
  atkPercentScaling: number;
  cardBaseDamage?: number;
  skillTypeTag?: string;
  effects: SkillEffect[];
}

export interface Awakener {
  id: number;
  name: string;
  baseAtk: number;
  baseDef: number;
  baseHp: number;
  tags: string[];
  skills: Skill[];
}

// ── Request types ─────────────────────────────────────────────────────────────

export interface TargetRequest {
  name?: string;
  def: number;
  shieldHp?: number;
  defFactor: number;
  tags: string[];
}

export interface ExtraEffect {
  type: string;
  value: number;
  conditionTag?: string;
}

export interface DamageCalculationRequest {
  awakenerId: number;
  skillId: number;
  isCritical: boolean;
  baseCritMultiplier: number;
  target: TargetRequest;
  extraEffects: ExtraEffect[];
}

// ── Response types ─────────────────────────────────────────────────────────────

export interface PipelineStep {
  layerName: string;
  damageBefore: number;
  damageAfter: number;
  description: string;
}

export interface DamageResult {
  displayedDamage: number;
  finalDamage: number;
  isCritical: boolean;
  critMultiplierApplied: number;
  shieldDamage?: number;
  overflowDamage?: number;
  hpDamage?: number;
  shieldBroken: boolean;
  breakdown: PipelineStep[];
}
