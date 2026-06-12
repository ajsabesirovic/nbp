import { useEffect, useMemo, useState } from 'react';
import { Link, useNavigate, useSearchParams } from 'react-router-dom';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import toast from 'react-hot-toast';
import { api } from '../api/client';
import { useAuth } from '../context/AuthContext';
import { SET_METRICS, metricsForType } from '../lib/setMetrics';

export default function SessionLogger() {
  const nav = useNavigate();
  const qc = useQueryClient();
  const { user } = useAuth();
  const activePlanId = user?.activePlanId || '';
  const [searchParams] = useSearchParams();

  const [planId, setPlanId] = useState(searchParams.get('planId') || activePlanId);

  const [startedAt] = useState(() => new Date());
  const [exercises, setExercises] = useState([]);
  const [notes, setNotes] = useState('');
  const [feeling, setFeeling] = useState(4);
  const [dayNumber, setDayNumber] = useState(Number(searchParams.get('dayNumber')) || 1);

  const { data: exerciseLib } = useQuery({
    queryKey: ['exercises-pick'],
    queryFn: () => api.get('/exercises?limit=200').then((r) => r.data),
  });

  const { data: plan } = useQuery({
    queryKey: ['plan', planId],
    queryFn: () => api.get(`/plans/${planId}`).then((r) => r.data),
    enabled: !!planId,
  });

  const { data: activePlan } = useQuery({
    queryKey: ['plan', activePlanId],
    queryFn: () => api.get(`/plans/${activePlanId}`).then((r) => r.data),
    enabled: !!activePlanId,
  });

  const chooseWorkoutType = (value) => {
    setPlanId(value);
    setDayNumber(1);
    if (!value) setExercises([]);
  };

  useEffect(() => {
    if (plan?.days?.length) {
      const day = plan.days.find((d) => d.dayNumber === dayNumber) || plan.days[0];
      setExercises(
        day.exercises.map((ex) => ({
          exerciseId: ex.exerciseId,
          nameSnapshot: ex.nameSnapshot,

          type: exerciseLib?.items?.find((e) => e._id === ex.exerciseId)?.type,
          sets: Array.from({ length: ex.sets || 3 }, (_, i) => ({
            setNumber: i + 1,
            completed: true,
          })),
        })),
      );
    }
  }, [plan, dayNumber, exerciseLib]);

  const totalVolume = useMemo(
    () =>
      exercises.reduce(
        (sum, ex) =>
          sum + ex.sets.reduce((s, set) => s + (set.completed ? (set.weightKg || 0) * (set.reps || 0) : 0), 0),
        0,
      ),
    [exercises],
  );

  const addExercise = (exerciseId) => {
    const lib = exerciseLib?.items.find((e) => e._id === exerciseId);
    if (!lib) return;
    if (exercises.some((ex) => ex.exerciseId === exerciseId)) {
      toast.error(`${lib.name} is already in this workout — add more sets to it instead.`);
      return;
    }
    setExercises([
      ...exercises,
      {
        exerciseId,
        nameSnapshot: lib.name,
        type: lib.type,
        sets: [{ setNumber: 1, completed: true }],
      },
    ]);
  };

  const updateSet = (ei, si, patch) => {
    setExercises(
      exercises.map((ex, i) =>
        i === ei
          ? { ...ex, sets: ex.sets.map((s, j) => (j === si ? { ...s, ...patch } : s)) }
          : ex,
      ),
    );
  };

  const addSet = (ei) => {
    setExercises(
      exercises.map((ex, i) =>
        i === ei
          ? {
              ...ex,
              sets: [
                ...ex.sets,
                {

                  ...metricsForType(ex.type).reduce(
                    (acc, c) => ({ ...acc, [c]: ex.sets.at(-1)?.[c] }),
                    {},
                  ),
                  setNumber: ex.sets.length + 1,
                  completed: true,
                },
              ],
            }
          : ex,
      ),
    );
  };

  const removeExercise = (ei) => setExercises(exercises.filter((_, i) => i !== ei));

  const save = useMutation({
    mutationFn: (payload) => api.post('/sessions', payload),
    onSuccess: () => {

      qc.invalidateQueries({ queryKey: ['sessions'] });
      qc.invalidateQueries({ queryKey: ['recentSessions'] });
      qc.invalidateQueries({ queryKey: ['streak'] });
      qc.invalidateQueries({ queryKey: ['weekly'] });
      qc.invalidateQueries({ queryKey: ['muscle'] });
      qc.invalidateQueries({ queryKey: ['progression'] });
      qc.invalidateQueries({ queryKey: ['prs'] });
      toast.success('Workout logged!');
      nav('/sessions');
    },
    onError: (err) => toast.error(err.response?.data?.error || 'Failed'),
  });

  const finish = () => {
    save.mutate({
      planId: planId || undefined,
      startedAt,
      endedAt: new Date(),
      exercises,
      notes,
      feeling: Number(feeling),
    });
  };

  return (
    <div className="space-y-4">
      <div className="flex justify-between items-center">
        <h1 className="text-2xl font-bold">
          {plan ? `Logging: ${plan.name}` : 'Free workout'}
        </h1>
        <div className="text-sm text-slate-500">
          Total volume: <span className="font-semibold text-slate-900">{Math.round(totalVolume)} kg</span>
        </div>
      </div>

      <div className="card">
        <label className="label">Workout type</label>
        <select
          className="input max-w-md"
          value={planId}
          onChange={(e) => chooseWorkoutType(e.target.value)}
        >
          <option value="">Free workout</option>
          {activePlanId && (
            <option value={activePlanId}>
              {activePlan
                ? `${activePlan.name} (active plan)`
                : 'Active plan'}
            </option>
          )}
        </select>
        {!activePlanId && (
          <p className="text-xs text-slate-400 mt-1">
            No active plan set. Log a free workout, or pick an active plan on the{' '}
            <Link to="/plans" className="text-brand-600">
              Plans
            </Link>{' '}
            page.
          </p>
        )}
      </div>

      {plan && plan.days?.length > 1 && (
        <div className="card">
          <label className="label">Workout day</label>
          <select
            className="input max-w-xs"
            value={dayNumber}
            onChange={(e) => setDayNumber(Number(e.target.value))}
          >
            {plan.days.map((d) => (
              <option key={d.dayNumber} value={d.dayNumber}>
                Day {d.dayNumber} — {d.name}
              </option>
            ))}
          </select>
        </div>
      )}

      {exercises.map((ex, ei) => {
        const cols = metricsForType(ex.type);
        return (
          <div key={ei} className="card">
            <div className="flex justify-between items-center mb-2">
              <h3 className="font-semibold">{ex.nameSnapshot}</h3>
              <button className="text-red-500 text-xs" onClick={() => removeExercise(ei)}>
                remove
              </button>
            </div>
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
                {ex.sets.map((s, si) => (
                  <tr key={si}>
                    <td className="py-1">{si + 1}</td>
                    {cols.map((c) => (
                      <td key={c}>
                        <input
                          type="number"
                          min="0"
                          className={`input !py-1 ${SET_METRICS[c].cls}`}
                          value={SET_METRICS[c].toView(s[c])}
                          onChange={(e) => updateSet(ei, si, { [c]: SET_METRICS[c].fromView(e.target.value) })}
                        />
                      </td>
                    ))}
                    <td>
                      <input
                        type="number"
                        min="1"
                        max="10"
                        className="input !py-1 w-16"
                        value={s.rpe || ''}
                        onChange={(e) => updateSet(ei, si, { rpe: Number(e.target.value) })}
                      />
                    </td>
                    <td>
                      <input
                        type="checkbox"
                        className="w-5 h-5"
                        checked={!!s.completed}
                        onChange={(e) => updateSet(ei, si, { completed: e.target.checked })}
                      />
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
            <button className="btn-secondary text-xs mt-2" onClick={() => addSet(ei)}>
              + Add set
            </button>
          </div>
        );
      })}

      <div className="card">
        <label className="label">Add exercise</label>
        <select
          className="input max-w-md"
          value=""
          onChange={(e) => {
            addExercise(e.target.value);
            e.target.value = '';
          }}
        >
          <option value="">Pick an exercise...</option>
          {exerciseLib?.items
            ?.filter((e) => !exercises.some((ex) => ex.exerciseId === e._id))
            .map((e) => (
              <option key={e._id} value={e._id}>
                {e.name}
              </option>
            ))}
        </select>
      </div>

      <div className="card space-y-3">
        <div>
          <label className="label">Notes</label>
          <textarea className="input" rows="3" value={notes} onChange={(e) => setNotes(e.target.value)} />
        </div>
        <div>
          <label className="label">Feeling (1–5)</label>
          <input
            type="range"
            min="1"
            max="5"
            value={feeling}
            onChange={(e) => setFeeling(e.target.value)}
            className="w-full"
          />
          <div className="text-sm text-slate-500">Rated: {feeling}/5</div>
        </div>
      </div>

      <div className="flex gap-2 justify-end">
        <button className="btn-secondary" onClick={() => nav('/sessions')}>
          Cancel
        </button>
        <button className="btn-primary" onClick={finish} disabled={save.isPending || !exercises.length}>
          {save.isPending ? 'Saving...' : 'Finish workout'}
        </button>
      </div>
    </div>
  );
}
