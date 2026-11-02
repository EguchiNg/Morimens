import { useEffect, useState } from 'react';
import { api } from '../api/client';
import type { Awakener, DamageResult, Skill } from '../types';
import BreakdownTable from './BreakdownTable';

export default function DamageCalculator() {
  const [awakeners, setAwakeners] = useState<Awakener[]>([]);
  const [awakener,  setAwakener]  = useState<Awakener  | null>(null);
  const [skill,     setSkill]     = useState<Skill     | null>(null);
  const [result,    setResult]    = useState<DamageResult | null>(null);
  const [error,     setError]     = useState<string | null>(null);
  const [loading,   setLoading]   = useState(false);

  // form state
  const [def,       setDef]       = useState('0');
  const [shieldHp,  setShieldHp]  = useState('0');
  const [defFactor, setDefFactor] = useState('1.0');
  const [isCrit,    setIsCrit]    = useState(false);
  const [critMult,  setCritMult]  = useState('1.5');

  useEffect(() => {
    api.getAwakeners()
      .then((data) => {
        setAwakeners(data);
        if (data.length > 0) {
          setAwakener(data[0]);
          if (data[0].skills.length > 0) setSkill(data[0].skills[0]);
        }
      })
      .catch((e) => setError(String(e)));
  }, []);

  const handleAwakenerChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    const found = awakeners.find((a) => a.id === Number(e.target.value)) ?? null;
    setAwakener(found);
    setSkill(found?.skills[0] ?? null);
    setResult(null);
  };

  const handleSkillChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    const found = awakener?.skills.find((s) => s.id === Number(e.target.value)) ?? null;
    setSkill(found);
    setResult(null);
  };

  const handleCalculate = async () => {
    if (!awakener || !skill) return;
    setLoading(true);
    setError(null);
    try {
      const res = await api.calculate({
        awakenerId: awakener.id,
        skillId: skill.id,
        isCritical: isCrit,
        baseCritMultiplier: Number(critMult) || 1.5,
        target: {
          def: Number(def) || 0,
          shieldHp: Number(shieldHp) > 0 ? Number(shieldHp) : undefined,
          defFactor: Number(defFactor) || 1.0,
          tags: [],
        },
        extraEffects: [],
      });
      setResult(res);
    } catch (e) {
      setError(String(e));
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="calculator">
      <header>
        <h1>Morimens Damage Calculator</h1>
        <p className="subtitle">Select an Awakener and Skill, configure the target, and calculate.</p>
      </header>

      <div className="layout">
        {/* ── Input panel ── */}
        <aside className="inputs">
          <section>
            <label htmlFor="awakener-select">Awakener</label>
            <select id="awakener-select" value={awakener?.id ?? ''} onChange={handleAwakenerChange}>
              {awakeners.map((a) => (
                <option key={a.id} value={a.id}>{a.name}</option>
              ))}
            </select>

            <label htmlFor="skill-select">Skill</label>
            <select id="skill-select" value={skill?.id ?? ''} onChange={handleSkillChange}>
              {(awakener?.skills ?? []).map((s) => (
                <option key={s.id} value={s.id}>{s.name}</option>
              ))}
            </select>

            {skill && (
              <p className="skill-desc">{skill.description}</p>
            )}
          </section>

          <section>
            <label htmlFor="def-input">Target DEF</label>
            <input id="def-input" type="number" value={def}
                   onChange={(e) => setDef(e.target.value)} />

            <label htmlFor="shield-input">Target Shield HP <span className="hint">(0 = none)</span></label>
            <input id="shield-input" type="number" value={shieldHp}
                   onChange={(e) => setShieldHp(e.target.value)} />

            <label htmlFor="deffactor-input">DEF Factor</label>
            <input id="deffactor-input" type="number" step="0.01" value={defFactor}
                   onChange={(e) => setDefFactor(e.target.value)} />
          </section>

          <section>
            <label className="checkbox-label">
              <input type="checkbox" checked={isCrit} onChange={(e) => setIsCrit(e.target.checked)} />
              Critical Hit
            </label>

            <label htmlFor="critmult-input">Base Crit Multiplier</label>
            <input id="critmult-input" type="number" step="0.1" value={critMult}
                   onChange={(e) => setCritMult(e.target.value)} />
          </section>

          <button className="calc-btn" onClick={handleCalculate} disabled={loading || !skill}>
            {loading ? 'Calculating…' : 'Calculate'}
          </button>

          {error && <p className="error">{error}</p>}
        </aside>

        {/* ── Results ── */}
        <main className="results">
          {result ? (
            <>
              <div className="stat-tiles">
                <div className="tile">
                  <div className="tile-label">Displayed Damage</div>
                  <div className="tile-value">{result.displayedDamage.toLocaleString()}</div>
                </div>
                <div className="tile accent">
                  <div className="tile-label">Actual Damage {result.isCritical ? '⚡ CRIT' : ''}</div>
                  <div className="tile-value">{result.finalDamage.toLocaleString()}</div>
                </div>
                {result.shieldBroken && (
                  <div className="tile warn">
                    <div className="tile-label">Shield Broken!</div>
                    <div className="tile-value">+{result.overflowDamage?.toLocaleString()} overflow</div>
                  </div>
                )}
              </div>

              <BreakdownTable steps={result.breakdown} />
            </>
          ) : (
            <div className="empty-state">
              <p>Configure your inputs and click <strong>Calculate</strong> to see the damage breakdown.</p>
            </div>
          )}
        </main>
      </div>
    </div>
  );
}
