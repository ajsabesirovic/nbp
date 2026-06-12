import { useState } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import toast from 'react-hot-toast';
import { Trash2 } from 'lucide-react';
import { api } from '../api/client';
import IconButton from '../components/IconButton';

const emptyForm = () => ({
  recordedAt: new Date().toISOString().slice(0, 10),
  weightKg: '',
  waistCm: '',
  chestCm: '',
  armCm: '',
  thighCm: '',
  bodyFatPct: '',
  note: '',
});

const num = (v) => (v === '' ? undefined : Number(v));

export default function Measurements() {
  const qc = useQueryClient();
  const [form, setForm] = useState(emptyForm());

  const { data, isLoading } = useQuery({
    queryKey: ['measurements'],
    queryFn: () => api.get('/measurements').then((r) => r.data),
  });

  const create = useMutation({
    mutationFn: (payload) => api.post('/measurements', payload),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['measurements'] });
      toast.success('Saved');
      setForm(emptyForm());
    },
  });

  const del = useMutation({
    mutationFn: (id) => api.delete(`/measurements/${id}`),
    onSuccess: () => qc.invalidateQueries({ queryKey: ['measurements'] }),
  });

  const submit = (e) => {
    e.preventDefault();
    create.mutate({
      recordedAt: form.recordedAt,
      weightKg: num(form.weightKg),
      waistCm: num(form.waistCm),
      chestCm: num(form.chestCm),
      armCm: num(form.armCm),
      thighCm: num(form.thighCm),
      bodyFatPct: num(form.bodyFatPct),
      note: form.note || undefined,
    });
  };

  const upd = (k) => (e) => setForm({ ...form, [k]: e.target.value });

  return (
    <div className="space-y-4 max-w-4xl">
      <h1 className="text-2xl font-bold">Body measurements</h1>

      <form onSubmit={submit} className="card grid grid-cols-2 md:grid-cols-4 gap-3">
        <div>
          <label className="label">Date</label>
          <input type="date" className="input" value={form.recordedAt} onChange={upd('recordedAt')} />
        </div>
        <div>
          <label className="label">Weight (kg)</label>
          <input type="number" step="0.1" className="input" value={form.weightKg} onChange={upd('weightKg')} />
        </div>
        <div>
          <label className="label">Waist (cm)</label>
          <input type="number" step="0.1" className="input" value={form.waistCm} onChange={upd('waistCm')} />
        </div>
        <div>
          <label className="label">Chest (cm)</label>
          <input type="number" step="0.1" className="input" value={form.chestCm} onChange={upd('chestCm')} />
        </div>
        <div>
          <label className="label">Arm (cm)</label>
          <input type="number" step="0.1" className="input" value={form.armCm} onChange={upd('armCm')} />
        </div>
        <div>
          <label className="label">Thigh (cm)</label>
          <input type="number" step="0.1" className="input" value={form.thighCm} onChange={upd('thighCm')} />
        </div>
        <div>
          <label className="label">Body fat %</label>
          <input type="number" step="0.1" className="input" value={form.bodyFatPct} onChange={upd('bodyFatPct')} />
        </div>
        <div>
          <label className="label">Note</label>
          <input className="input" value={form.note} onChange={upd('note')} />
        </div>
        <div className="col-span-2 md:col-span-4">
          <button className="btn-primary" disabled={create.isPending}>
            {create.isPending ? 'Saving...' : 'Add measurement'}
          </button>
        </div>
      </form>

      <div className="card">
        <h2 className="font-semibold mb-2">History</h2>
        {isLoading ? (
          <div>Loading...</div>
        ) : (
          <table className="w-full text-sm">
            <thead className="text-xs uppercase text-slate-500 border-b border-slate-100">
              <tr>
                <th className="text-left py-1">Date</th>
                <th>Weight</th>
                <th>Waist</th>
                <th>Chest</th>
                <th>Arm</th>
                <th>Thigh</th>
                <th>BF%</th>
                <th>Note</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              {data?.items?.map((m) => (
                <tr key={m._id} className="border-b border-slate-50">
                  <td className="py-1">{new Date(m.recordedAt).toLocaleDateString()}</td>
                  <td className="text-center">{m.weightKg ?? '—'}</td>
                  <td className="text-center">{m.waistCm ?? '—'}</td>
                  <td className="text-center">{m.chestCm ?? '—'}</td>
                  <td className="text-center">{m.armCm ?? '—'}</td>
                  <td className="text-center">{m.thighCm ?? '—'}</td>
                  <td className="text-center">{m.bodyFatPct ?? '—'}</td>
                  <td className="text-xs text-slate-500">{m.note}</td>
                  <td>
                    <IconButton
                      icon={Trash2}
                      label="Delete measurement"
                      variant="danger"
                      size={16}
                      onClick={() => confirm('Delete?') && del.mutate(m._id)}
                    />
                  </td>
                </tr>
              ))}
              {!data?.items?.length && (
                <tr>
                  <td colSpan={9} className="text-slate-400 py-2">No measurements yet.</td>
                </tr>
              )}
            </tbody>
          </table>
        )}
      </div>
    </div>
  );
}
