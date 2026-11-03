import { useEffect, useState } from 'react';
import { simulationApi } from '../api/auth';
import { useAuth } from '../context/AuthContext';
import type { SavedSimulation } from '../types/auth';

interface Props {
  /** Called when the user wants to load a simulation into the calculator. */
  onLoad: (sim: SavedSimulation) => void;
}

export default function SavedSimulations({ onLoad }: Props) {
  const { tokens } = useAuth();
  const [sims,    setSims]    = useState<SavedSimulation[]>([]);
  const [loading, setLoading] = useState(true);
  const [error,   setError]   = useState<string | null>(null);

  const fetchSims = async () => {
    if (!tokens) return;
    try {
      const data = await simulationApi.list(tokens.accessToken);
      setSims(data);
    } catch (e) {
      setError(String(e));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { fetchSims(); }, [tokens]);

  const handleDelete = async (id: number) => {
    if (!tokens) return;
    await simulationApi.delete(tokens.accessToken, id);
    setSims(prev => prev.filter(s => s.id !== id));
  };

  if (!tokens) return null;
  if (loading) return <p className="sims-loading">Loading saved simulations…</p>;
  if (error)   return <p className="auth-error">{error}</p>;

  return (
    <section className="saved-sims">
      <h3>Saved Simulations</h3>
      {sims.length === 0 ? (
        <p className="sims-empty">No saved simulations yet. Run a calculation and save it!</p>
      ) : (
        <ul className="sims-list">
          {sims.map(s => (
            <li key={s.id} className="sim-row">
              <div className="sim-info">
                <span className="sim-name">{s.name}</span>
                <span className="sim-date">{new Date(s.updatedAt).toLocaleDateString()}</span>
              </div>
              <div className="sim-actions">
                <button className="sim-load-btn" onClick={() => onLoad(s)}>Load</button>
                <button className="sim-del-btn"  onClick={() => handleDelete(s.id)} aria-label="Delete">✕</button>
              </div>
            </li>
          ))}
        </ul>
      )}
    </section>
  );
}
