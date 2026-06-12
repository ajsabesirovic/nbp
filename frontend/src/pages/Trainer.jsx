import { useState } from 'react';
import { Link } from 'react-router-dom';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import toast from 'react-hot-toast';
import { api } from '../api/client';
import { useAuth } from '../context/AuthContext';

const prettyLabel = (v) =>
  v ? v.replace(/_/g, ' ').replace(/^./, (c) => c.toUpperCase()) : '—';

export default function Trainer() {
  const { user } = useAuth();

  if (user?.role === 'admin') return <AdminTrainerRoster />;
  return <TrainerDashboard />;
}

function TrainerDashboard() {
  const [activeClient, setActiveClient] = useState(null);

  const clients = useQuery({
    queryKey: ['trainer-clients'],
    queryFn: () => api.get('/trainer/clients').then((r) => r.data),
  });

  const summary = useQuery({
    queryKey: ['trainer-client-summary', activeClient],
    queryFn: () => api.get(`/trainer/clients/${activeClient}/summary`).then((r) => r.data),
    enabled: !!activeClient,
  });

  return (
    <div className="space-y-4">
      <h1 className="text-2xl font-bold">Trainer dashboard</h1>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-4 items-start">
        <div className="card">
          <h2 className="font-semibold mb-3">My clients</h2>
          <ul className="divide-y divide-slate-200">
            {(clients.data?.items || []).map((c) => (
              <li key={c._id}>
                <button
                  className={`w-full text-left py-2 px-2 rounded ${
                    activeClient === c._id ? 'bg-brand-50 text-brand-700' : 'hover:bg-slate-50'
                  }`}
                  onClick={() => setActiveClient(c._id)}
                >
                  <div className="font-medium">{c.name}</div>
                  <div className="text-xs text-slate-500">{c.email}</div>
                </button>
              </li>
            ))}
            {!clients.data?.items?.length && (
              <li className="py-4 text-sm text-slate-400">No clients yet.</li>
            )}
          </ul>
        </div>

        <div className="lg:col-span-2 space-y-4">
          {!activeClient && (
            <div className="card text-slate-500 text-sm">Pick a client to see their progress.</div>
          )}
          {summary.data && (
            <>
              <div className="card">
                <h2 className="font-semibold mb-1">{summary.data.client.name}</h2>
                <div className="text-sm text-slate-500">
                  Goal: {prettyLabel(summary.data.profile?.goal)} · Level:{' '}
                  {prettyLabel(summary.data.profile?.experience)}
                </div>
              </div>

              <div className="card">
                <h3 className="font-semibold mb-3">Plan completion</h3>
                {summary.data.completion?.length ? (
                  <ul className="space-y-2">
                    {summary.data.completion.map((c) => {

                      const cycles = Math.floor(c.loggedSessions / c.expectedSessions);
                      const rem = c.loggedSessions % c.expectedSessions;
                      const cyclesLabel = `${cycles} cycle${cycles > 1 ? 's' : ''} completed`;
                      const label =
                        cycles < 1
                          ? `${c.loggedSessions}/${c.expectedSessions} sessions`
                          : rem === 0
                            ? cyclesLabel
                            : `${cyclesLabel} · ${rem}/${c.expectedSessions}`;
                      const barPct =
                        cycles < 1
                          ? Math.round(c.completionRate * 100)
                          : rem === 0
                            ? 100
                            : Math.round((rem / c.expectedSessions) * 100);
                      return (
                        <li key={c._id} className="text-sm">
                          <div className="flex justify-between mb-1">
                            <span>{c.name}</span>
                            <span className="text-slate-500">{label}</span>
                          </div>
                          <div className="bg-slate-100 rounded-full h-2 overflow-hidden">
                            <div className="bg-brand-600 h-2" style={{ width: `${barPct}%` }} />
                          </div>
                        </li>
                      );
                    })}
                  </ul>
                ) : (
                  <div className="text-sm text-slate-400">No assigned plans.</div>
                )}
              </div>

              <div className="card">
                <h3 className="font-semibold mb-3">Recent sessions</h3>
                <ul className="divide-y divide-slate-200">
                  {summary.data.recentSessions.map((s) => (
                    <li key={s._id}>
                      <Link
                        to={`/trainer/clients/${activeClient}/sessions/${s._id}`}
                        className="py-2 flex justify-between text-sm hover:bg-slate-50 rounded px-2 -mx-2"
                      >
                        <span>{new Date(s.startedAt).toLocaleDateString()}</span>
                        <span className="text-slate-500">
                          {s.exercises.length} ex · {Math.round(s.totalVolumeKg)} kg
                        </span>
                      </Link>
                    </li>
                  ))}
                </ul>
              </div>

              <ClientAllSessions clientId={activeClient} />

              {summary.data.photos?.length > 0 && (
                <div className="card">
                  <h3 className="font-semibold mb-3">Progress photos</h3>
                  <div className="grid grid-cols-3 sm:grid-cols-4 gap-2">
                    {summary.data.photos.map((p) => (
                      <a key={p._id} href={p.url} target="_blank" rel="noreferrer" title={p.note || ''}>
                        <img
                          src={p.url}
                          alt={p.note || 'Progress photo'}
                          className="w-full aspect-square object-cover rounded hover:opacity-90 transition-opacity"
                        />
                        <div className="text-[10px] text-slate-400 mt-0.5">
                          {new Date(p.takenAt).toLocaleDateString()}
                        </div>
                      </a>
                    ))}
                  </div>
                </div>
              )}

              <ClientMeasurements clientId={activeClient} />
            </>
          )}
        </div>
      </div>
    </div>
  );
}

function ClientAllSessions({ clientId }) {
  const [open, setOpen] = useState(false);
  const [page, setPage] = useState(1);
  const limit = 20;

  const { data, isLoading } = useQuery({
    queryKey: ['client-sessions', clientId, page],
    queryFn: () =>
      api.get(`/trainer/clients/${clientId}/sessions`, { params: { page, limit } }).then((r) => r.data),
    enabled: open,
  });

  const total = data?.total ?? 0;
  const totalPages = Math.max(1, Math.ceil(total / limit));

  return (
    <div className="card">
      <div className="flex items-center justify-between">
        <h3 className="font-semibold">All sessions</h3>
        <button
          type="button"
          className="btn-secondary !py-1 !px-3 text-xs"
          onClick={() => {
            setPage(1);
            setOpen((o) => !o);
          }}
        >
          {open ? 'Hide' : 'View all sessions'}
        </button>
      </div>

      {open && (
        <div className="mt-3">
          {isLoading ? (
            <div className="text-sm text-slate-400">Loading...</div>
          ) : (
            <>
              <div className="text-xs text-slate-400 mb-2">{total} sessions total</div>
              <ul className="divide-y divide-slate-200">
                {(data?.items || []).map((s) => (
                  <li key={s._id}>
                    <Link
                      to={`/trainer/clients/${clientId}/sessions/${s._id}`}
                      className="py-2 flex justify-between text-sm hover:bg-slate-50 rounded px-2 -mx-2"
                    >
                      <span>{new Date(s.startedAt).toLocaleDateString()}</span>
                      <span className="text-slate-500">
                        {s.exercises.length} ex · {s.completedSets} sets · {Math.round(s.totalVolumeKg)} kg
                      </span>
                    </Link>
                  </li>
                ))}
                {!data?.items?.length && <li className="py-2 text-sm text-slate-400">No sessions.</li>}
              </ul>
              {totalPages > 1 && (
                <div className="flex items-center justify-between mt-3 text-sm">
                  <button
                    type="button"
                    className="btn-secondary !py-1 !px-3 text-xs"
                    disabled={page <= 1}
                    onClick={() => setPage((p) => p - 1)}
                  >
                    Prev
                  </button>
                  <span className="text-slate-500">
                    Page {page} / {totalPages}
                  </span>
                  <button
                    type="button"
                    className="btn-secondary !py-1 !px-3 text-xs"
                    disabled={page >= totalPages}
                    onClick={() => setPage((p) => p + 1)}
                  >
                    Next
                  </button>
                </div>
              )}
            </>
          )}
        </div>
      )}
    </div>
  );
}

function AdminTrainerRoster() {
  const qc = useQueryClient();

  const { data, isLoading } = useQuery({
    queryKey: ['admin-trainers'],
    queryFn: () => api.get('/admin/trainers').then((r) => r.data),
  });

  const usersQ = useQuery({
    queryKey: ['admin-assignable-users'],
    queryFn: () =>
      api.get('/admin/users', { params: { role: 'user', limit: 1000 } }).then((r) => r.data),
  });

  const trainers = data?.items || [];
  const assignedIds = new Set(trainers.flatMap((t) => t.clients.map((c) => c._id)));
  const available = (usersQ.data?.items || []).filter((u) => !assignedIds.has(u._id));

  const assign = useMutation({
    mutationFn: ({ trainerId, clientId }) =>
      api.post(`/admin/trainers/${trainerId}/clients`, { clientId }),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['admin-trainers'] });
      toast.success('Client assigned');
    },
    onError: (err) => toast.error(err.response?.data?.detail || 'Failed to assign'),
  });

  const unassign = useMutation({
    mutationFn: ({ trainerId, clientId }) =>
      api.delete(`/admin/trainers/${trainerId}/clients/${clientId}`),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['admin-trainers'] });
      toast.success('Client removed');
    },
    onError: (err) => toast.error(err.response?.data?.detail || 'Failed to remove'),
  });

  return (
    <div className="space-y-4">
      <div>
        <h1 className="text-2xl font-bold">Trainers</h1>
        <p className="text-sm text-slate-500">
          Every trainer on the platform and the clients assigned to them.
        </p>
      </div>

      {isLoading ? (
        <div className="text-slate-400">Loading…</div>
      ) : (
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-4">
          {trainers.map((t) => (
            <AdminTrainerCard
              key={t.userId}
              trainer={t}
              available={available}
              busy={assign.isPending || unassign.isPending}
              onAssign={(clientId) => assign.mutate({ trainerId: t.userId, clientId })}
              onUnassign={(clientId) => unassign.mutate({ trainerId: t.userId, clientId })}
            />
          ))}
          {!trainers.length && <div className="text-slate-400">No trainers found.</div>}
        </div>
      )}
    </div>
  );
}

function AdminTrainerCard({ trainer: t, available, busy, onAssign, onUnassign }) {
  const [clientId, setClientId] = useState('');

  const submit = () => {
    if (!clientId) return;
    onAssign(clientId);
    setClientId('');
  };

  return (
    <div className="card">
      <div className="flex items-start justify-between gap-2">
        <div>
          <h3 className="font-semibold">{t.name}</h3>
          <div className="text-xs text-slate-500">{t.email}</div>
          {t.specialization && (
            <div className="text-xs text-slate-400 mt-0.5">{t.specialization}</div>
          )}
        </div>
        <span className="badge bg-brand-50 text-brand-700 shrink-0">
          {t.clientCount} {t.clientCount === 1 ? 'client' : 'clients'}
        </span>
      </div>

      <ul className="divide-y divide-slate-100 mt-3">
        {t.clients.map((c) => (
          <li key={c._id} className="py-2 flex items-center justify-between gap-2 text-sm">
            <span>
              {c.name} <span className="text-xs text-slate-400">{c.email}</span>
              <span className="text-xs text-slate-400"> · joined {new Date(c.joinedAt).toLocaleDateString()}</span>
            </span>
            <button
              className="btn-secondary text-xs shrink-0"
              disabled={busy}
              onClick={() => onUnassign(c._id)}
            >
              Unassign
            </button>
          </li>
        ))}
        {!t.clients.length && <li className="py-2 text-sm text-slate-400">No clients yet.</li>}
      </ul>

      <div className="flex gap-2 border-t border-slate-100 pt-3 mt-1">
        <select
          className="input flex-1 !text-sm !py-1"
          value={clientId}
          onChange={(e) => setClientId(e.target.value)}
        >
          <option value="">Assign a user…</option>
          {available.map((u) => (
            <option key={u._id} value={u._id}>
              {u.name} ({u.email})
            </option>
          ))}
        </select>
        <button className="btn-primary text-sm" disabled={!clientId || busy} onClick={submit}>
          Assign
        </button>
      </div>
      {!available.length && (
        <div className="text-xs text-slate-400 mt-1">No unassigned users available.</div>
      )}
    </div>
  );
}

const EMPTY_MEASUREMENT = {
  recordedAt: '',
  weightKg: '',
  waistCm: '',
  chestCm: '',
  armCm: '',
  thighCm: '',
  bodyFatPct: '',
  note: '',
};

function ClientMeasurements({ clientId }) {
  const qc = useQueryClient();
  const [form, setForm] = useState(EMPTY_MEASUREMENT);

  const { data } = useQuery({
    queryKey: ['client-measurements', clientId],
    queryFn: () => api.get(`/trainer/clients/${clientId}/measurements`).then((r) => r.data),
  });

  const create = useMutation({
    mutationFn: (payload) => api.post(`/trainer/clients/${clientId}/measurements`, payload),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['client-measurements', clientId] });
      toast.success('Measurement saved');
      setForm(EMPTY_MEASUREMENT);
    },
    onError: (err) => toast.error(err.response?.data?.detail || 'Failed to save'),
  });

  const items = data?.items || [];
  const upd = (k) => (e) => setForm((f) => ({ ...f, [k]: e.target.value }));
  const num = (v) => (v === '' ? null : Number(v));

  const submit = (e) => {
    e.preventDefault();
    create.mutate({
      recordedAt: form.recordedAt || null,
      weightKg: num(form.weightKg),
      waistCm: num(form.waistCm),
      chestCm: num(form.chestCm),
      armCm: num(form.armCm),
      thighCm: num(form.thighCm),
      bodyFatPct: num(form.bodyFatPct),
      note: form.note || null,
    });
  };

  return (
    <div className="card">
      <h3 className="font-semibold mb-1">Body measurements</h3>
      <p className="text-xs text-slate-400 mb-3">
        Recorded for your client — they can see these on their own progress page.
      </p>

      <form onSubmit={submit} className="grid grid-cols-2 sm:grid-cols-4 gap-2 mb-4">
        <input className="input" type="date" value={form.recordedAt} onChange={upd('recordedAt')} />
        <input className="input" type="number" step="0.1" placeholder="Weight kg" value={form.weightKg} onChange={upd('weightKg')} />
        <input className="input" type="number" step="0.1" placeholder="Waist cm" value={form.waistCm} onChange={upd('waistCm')} />
        <input className="input" type="number" step="0.1" placeholder="Chest cm" value={form.chestCm} onChange={upd('chestCm')} />
        <input className="input" type="number" step="0.1" placeholder="Arm cm" value={form.armCm} onChange={upd('armCm')} />
        <input className="input" type="number" step="0.1" placeholder="Thigh cm" value={form.thighCm} onChange={upd('thighCm')} />
        <input className="input" type="number" step="0.1" placeholder="Body fat %" value={form.bodyFatPct} onChange={upd('bodyFatPct')} />
        <input className="input" placeholder="Note" value={form.note} onChange={upd('note')} />
        <button className="btn-primary text-sm col-span-2 sm:col-span-4" disabled={create.isPending}>
          Add measurement
        </button>
      </form>

      {items.length ? (
        <div className="overflow-x-auto">
          <table className="w-full text-sm">
            <thead className="text-xs uppercase text-slate-500 border-b">
              <tr>
                <th className="text-left py-1">Date</th>
                <th>Weight</th>
                <th>Waist</th>
                <th>Chest</th>
                <th>Arm</th>
                <th>Thigh</th>
                <th>Fat %</th>
                <th className="text-left">Note</th>
              </tr>
            </thead>
            <tbody>
              {items.map((m) => (
                <tr key={m._id} className="border-b border-slate-100 text-center">
                  <td className="text-left py-1">{new Date(m.recordedAt).toLocaleDateString()}</td>
                  <td>{m.weightKg ?? '—'}</td>
                  <td>{m.waistCm ?? '—'}</td>
                  <td>{m.chestCm ?? '—'}</td>
                  <td>{m.armCm ?? '—'}</td>
                  <td>{m.thighCm ?? '—'}</td>
                  <td>{m.bodyFatPct ?? '—'}</td>
                  <td className="text-left text-slate-500">{m.note || ''}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      ) : (
        <p className="text-sm text-slate-400">No measurements yet.</p>
      )}
    </div>
  );
}
