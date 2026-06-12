import { SET_METRICS, metricsForType } from '../lib/setMetrics';

export default function SessionView({ session: s }) {
  return (
    <div className="space-y-4">
      <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
        <Stat label="Volume" value={`${Math.round(s.totalVolumeKg || 0)} kg`} />
        <Stat label="Completed sets" value={s.completedSets ?? 0} />
        <Stat label="Duration" value={`${Math.round((s.durationSec || 0) / 60)} min`} />
        <Stat label="Feeling" value={s.feeling ? `${s.feeling}/5` : '—'} />
      </div>

      {s.notes?.trim() && (
        <div className="card">
          <div className="label">Notes</div>
          <p className="text-sm whitespace-pre-wrap text-slate-700">{s.notes.trim()}</p>
        </div>
      )}

      {s.exercises?.map((ex, ei) => {
        const cols = metricsForType(ex.type);
        return (
          <div key={ei} className="card">
            <h3 className="font-semibold mb-2">{ex.nameSnapshot}</h3>
            <table className="w-full text-sm">
              <thead className="text-xs uppercase text-slate-500">
                <tr>
                  <th className="text-left">#</th>
                  {cols.map((c) => (
                    <th key={c} className="text-left">{SET_METRICS[c].label}</th>
                  ))}
                  <th className="text-left">RPE</th>
                  <th className="text-left">Done</th>
                </tr>
              </thead>
              <tbody>
                {ex.sets?.map((set, si) => (
                  <tr key={si} className={set.completed ? '' : 'text-slate-400'}>
                    <td className="py-1">{set.setNumber ?? si + 1}</td>
                    {cols.map((c) => (
                      <td key={c}>{SET_METRICS[c].display(set[c])}</td>
                    ))}
                    <td>{set.rpe ?? '—'}</td>
                    <td>{set.completed ? '✓' : '—'}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        );
      })}
    </div>
  );
}

function Stat({ label, value }) {
  return (
    <div className="card">
      <div className="text-xs uppercase text-slate-500">{label}</div>
      <div className="text-xl font-semibold mt-1">{value}</div>
    </div>
  );
}
