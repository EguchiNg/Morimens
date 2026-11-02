import type { PipelineStep } from '../types';

interface Props {
  steps: PipelineStep[];
}

export default function BreakdownTable({ steps }: Props) {
  if (steps.length === 0) return null;

  return (
    <div className="breakdown">
      <h3>Damage Breakdown</h3>
      <table>
        <thead>
          <tr>
            <th>Layer</th>
            <th className="num">Before</th>
            <th className="num">After</th>
            <th>Notes</th>
          </tr>
        </thead>
        <tbody>
          {steps.map((s) => (
            <tr key={s.layerName}>
              <td className="layer-name">{s.layerName}</td>
              <td className="num">{s.damageBefore.toLocaleString()}</td>
              <td className="num after">{s.damageAfter.toLocaleString()}</td>
              <td className="notes">{s.description}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
