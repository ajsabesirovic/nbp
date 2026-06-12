import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import toast from 'react-hot-toast';
import { api } from '../api/client';

const makeDays = (n, existing = []) => {
  const next = existing.slice(0, n).map((d, i) => ({ ...d, dayNumber: i + 1 }));
  while (next.length < n) {
    const k = next.length + 1;
    next.push({ dayNumber: k, name: `Day ${k}`, exercises: [] });
  }
  return next;
};

export default function PlanEditor() {
  const { id } = useParams();
  const nav = useNavigate();
  const qc = useQueryClient();
  const isEdit = !!id;

  const [form, setForm] = useState({
    name: '',
    description: '',
    durationWeeks: 8,
    level: 'beginner',
    goal: 'general_fitness',
    daysPerWeek: 3,
    visibility: 'private',
    status: 'published',
    days: makeDays(3),
  });

  const { data: existing } = useQuery({
    queryKey: ['plan', id],
    queryFn: () => api.get(`/plans/${id}`).then((r) => r.data),
    enabled: isEdit,
  });

  useEffect(() => {
    if (existing) {

      const days = existing.days?.length ? makeDays(existing.days.length, existing.days) : makeDays(1);
      setForm({ ...existing, days, daysPerWeek: days.length });
    }
  }, [existing]);

  const { data: exercisesList } = useQuery({
    queryKey: ['exercises-pick'],
    queryFn: () => api.get('/exercises?limit=200').then((r) => r.data),
  });

  const save = useMutation({
    mutationFn: (payload) => (isEdit ? api.patch(`/plans/${id}`, payload) : api.post('/plans', payload)),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['plans'] });
      qc.invalidateQueries({ queryKey: ['plan', id] });
      toast.success('Saved');
      nav('/plans');
    },
    onError: (err) => toast.error(err.response?.data?.error || 'Failed'),
  });

  const upd = (k) => (e) => setForm({ ...form, [k]: e.target.value });

  const applyDaysPerWeek = (val) => {
    const n = Math.min(7, Math.max(1, Math.floor(Number(val) || 1)));
    setForm((f) => ({ ...f, daysPerWeek: n, days: makeDays(n, f.days) }));
  };
  const addDay = () => applyDaysPerWeek(form.days.length + 1);
  const removeDay = (i) =>
    setForm((f) => {
      if (f.days.length <= 1) return f;
      const days = f.days.filter((_, idx) => idx !== i).map((d, idx) => ({ ...d, dayNumber: idx + 1 }));
      return { ...f, days, daysPerWeek: days.length };
    });

  const updateDay = (i, patch) =>
    setForm({ ...form, days: form.days.map((d, idx) => (idx === i ? { ...d, ...patch } : d)) });

  const addExercise = (i, exerciseId) => {
    if (!exerciseId) return;
    const ex = exercisesList?.items.find((e) => (e.id ?? e._id) === exerciseId);
    updateDay(i, {
      exercises: [
        ...form.days[i].exercises,
        { exerciseId, nameSnapshot: ex?.name, sets: 3, reps: '8-10', restSeconds: 90, alternateExerciseIds: [] },
      ],
    });
  };

  const updateExercise = (di, ei, patch) => {
    const day = form.days[di];
    const exercises = day.exercises.map((e, idx) => (idx === ei ? { ...e, ...patch } : e));
    updateDay(di, { exercises });
  };

  const submit = (e) => {
    e.preventDefault();
    save.mutate({
      ...form,
      durationWeeks: Number(form.durationWeeks),
      daysPerWeek: Number(form.daysPerWeek),
    });
  };

  return (
    <form onSubmit={submit} className="space-y-6">
      <h1 className="text-2xl font-bold">{isEdit ? 'Edit plan' : 'New plan'}</h1>

      <div className="card space-y-3">
        <div>
          <label className="label">Name</label>
          <input className="input" value={form.name} onChange={upd('name')} required />
        </div>
        <div>
          <label className="label">Description</label>
          <textarea className="input" rows="2" value={form.description} onChange={upd('description')} />
        </div>
        <div className="grid grid-cols-2 md:grid-cols-5 gap-3">
          <div>
            <label className="label">Duration (weeks)</label>
            <input className="input" type="number" min="1" value={form.durationWeeks} onChange={upd('durationWeeks')} />
          </div>
          <div>
            <label className="label">Days/week</label>
            <input
              className="input"
              type="number"
              min="1"
              max="7"
              value={form.daysPerWeek}
              onFocus={(e) => e.target.select()}
              onChange={(e) => setForm({ ...form, daysPerWeek: e.target.value })}
              onBlur={(e) => applyDaysPerWeek(e.target.value)}
            />
          </div>
          <div>
            <label className="label">Level</label>
            <select className="input" value={form.level} onChange={upd('level')}>
              <option>beginner</option>
              <option>intermediate</option>
              <option>advanced</option>
            </select>
          </div>
          <div>
            <label className="label">Goal</label>
            <select className="input" value={form.goal} onChange={upd('goal')}>
              <option value="weight_loss">weight_loss</option>
              <option value="muscle_gain">muscle_gain</option>
              <option value="endurance">endurance</option>
              <option value="general_fitness">general_fitness</option>
            </select>
          </div>
          <div>
            <label className="label">Visibility</label>
            <select className="input" value={form.visibility} onChange={upd('visibility')}>
              <option>private</option>
              <option>public</option>
            </select>
          </div>
        </div>
      </div>

      {form.days.map((day, di) => (
        <div key={di} className="card">
          <div className="flex justify-between items-center mb-3">
            <input
              className="input max-w-xs font-semibold"
              value={day.name}
              onChange={(e) => updateDay(di, { name: e.target.value })}
            />
            <button type="button" className="btn-danger text-xs" onClick={() => removeDay(di)}>
              Remove day
            </button>
          </div>
          <table className="w-full text-sm">
            <thead className="text-xs text-slate-500 uppercase">
              <tr>
                <th className="text-left py-1">Exercise</th>
                <th className="text-left py-1">Sets</th>
                <th className="text-left py-1">Reps</th>
                <th className="text-left py-1">Rest (s)</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              {day.exercises.map((ex, ei) => {
                const alts = ex.alternateExerciseIds || [];
                const altLib = exercisesList?.items || [];
                const altName = (aid) => altLib.find((e) => (e.id ?? e._id) === aid)?.name || aid;
                return (
                  <>
                    <tr key={`${ei}-main`} className="border-t border-slate-100">
                      <td className="py-1 pr-2">{ex.nameSnapshot || ex.exerciseId}</td>
                      <td className="py-1 pr-2">
                        <input
                          className="input !py-1 w-20"
                          type="number"
                          value={ex.sets || ''}
                          onChange={(e) => updateExercise(di, ei, { sets: Number(e.target.value) })}
                        />
                      </td>
                      <td className="py-1 pr-2">
                        <input
                          className="input !py-1 w-24"
                          value={ex.reps || ''}
                          onChange={(e) => updateExercise(di, ei, { reps: e.target.value })}
                        />
                      </td>
                      <td className="py-1 pr-2">
                        <input
                          className="input !py-1 w-20"
                          type="number"
                          value={ex.restSeconds || ''}
                          onChange={(e) => updateExercise(di, ei, { restSeconds: Number(e.target.value) })}
                        />
                      </td>
                      <td>
                        <button
                          type="button"
                          className="text-red-500 text-xs"
                          onClick={() =>
                            updateDay(di, { exercises: day.exercises.filter((_, i) => i !== ei) })
                          }
                        >
                          remove
                        </button>
                      </td>
                    </tr>
                    <tr key={`${ei}-alt`} className="text-xs text-slate-500">
                      <td colSpan={5} className="pb-2">
                        <span className="mr-2">Alternates:</span>
                        {alts.map((aid) => (
                          <span key={aid} className="inline-flex items-center bg-slate-100 rounded-full px-2 py-0.5 mr-1 mb-1">
                            {altName(aid)}
                            <button
                              type="button"
                              className="ml-1 text-slate-400 hover:text-red-500"
                              onClick={() =>
                                updateExercise(di, ei, {
                                  alternateExerciseIds: alts.filter((a) => a !== aid),
                                })
                              }
                            >
                              ×
                            </button>
                          </span>
                        ))}
                        <select
                          className="input !py-0.5 !text-xs inline-block w-auto max-w-xs ml-1"
                          value=""
                          onChange={(e) => {
                            const v = e.target.value;
                            if (!v || alts.includes(v) || v === ex.exerciseId) return;
                            updateExercise(di, ei, { alternateExerciseIds: [...alts, v] });
                            e.target.value = '';
                          }}
                        >
                          <option value="">+ alternate...</option>
                          {altLib
                            .filter((opt) => {
                              const oid = opt.id ?? opt._id;
                              return oid !== ex.exerciseId && !alts.includes(oid);
                            })
                            .map((opt) => {
                              const oid = opt.id ?? opt._id;
                              return (
                                <option key={oid} value={oid}>
                                  {opt.name}
                                </option>
                              );
                            })}
                        </select>
                      </td>
                    </tr>
                  </>
                );
              })}
            </tbody>
          </table>
          <select
            className="input mt-3 max-w-md"
            value=""
            onChange={(e) => {
              addExercise(di, e.target.value);
              e.target.value = '';
            }}
          >
            <option value="">+ Add exercise...</option>
            {exercisesList?.items?.map((e) => {
              const eid = e.id ?? e._id;
              return (
                <option key={eid} value={eid}>
                  {e.name}
                </option>
              );
            })}
          </select>
        </div>
      ))}

      <div className="flex gap-2">
        <button
          type="button"
          className="btn-secondary"
          onClick={addDay}
          disabled={form.days.length >= 7}
          title={form.days.length >= 7 ? 'A plan can have at most 7 days/week' : undefined}
        >
          + Add workout day
        </button>
        <button className="btn-primary" disabled={save.isPending}>
          {save.isPending ? 'Saving...' : 'Save plan'}
        </button>
      </div>
    </form>
  );
}
