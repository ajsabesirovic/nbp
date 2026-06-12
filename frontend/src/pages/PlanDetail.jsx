import { useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import toast from 'react-hot-toast';
import { api } from '../api/client';
import { useAuth } from '../context/AuthContext';

export default function PlanDetail() {
  const { id } = useParams();
  const { user } = useAuth();

  const { data: plan, isLoading, isError, error } = useQuery({
    queryKey: ['plan-detail', id],
    queryFn: () => api.get(`/plans/${id}`).then((r) => r.data),
    retry: false,
  });

  if (isLoading) return <div className="text-slate-400">Loading…</div>;
  if (isError) {
    const status = error?.response?.status;
    return (
      <div className="space-y-3">
        <Link to="/plans" className="text-sm text-brand-600">← Back to plans</Link>
        <div className="card text-slate-500">
          {status === 403 ? "You don't have access to this plan." : 'Plan not found.'}
        </div>
      </div>
    );
  }

  return (
    <div className="space-y-4">
      <Link to="/plans" className="text-sm text-brand-600">← Back to plans</Link>

      <div className="flex items-start justify-between gap-2">
        <div>
          <h1 className="text-2xl font-bold">{plan.name}</h1>
          {plan.description && <p className="text-slate-500">{plan.description}</p>}
          <div className="text-xs text-slate-400 mt-1">
            {plan.durationWeeks ? `${plan.durationWeeks} wk · ` : ''}
            {plan.daysPerWeek ? `${plan.daysPerWeek}×/wk · ` : ''}
            {plan.level} · {plan.goal} · by {plan.authorName}
          </div>
        </div>
        <span className="badge bg-brand-50 text-brand-700">{plan.visibility}</span>
      </div>

      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
        {(plan.days || []).map((d) => (
          <div key={d.dayNumber} className="card flex flex-col">
            <div className="mb-2">
              <div className="text-xs font-semibold uppercase tracking-wide text-brand-600">
                Day {d.dayNumber}
              </div>
              {d.name && <h3 className="font-semibold">{d.name}</h3>}
            </div>
            {d.exercises?.length ? (
              <ul className="divide-y divide-slate-100">
                {d.exercises.map((ex, i) => (
                  <li key={i} className="py-2 flex justify-between gap-2 text-sm">
                    <span>{ex.nameSnapshot || 'Exercise'}</span>
                    <span className="text-slate-500 whitespace-nowrap">
                      {ex.sets} × {ex.reps || '—'}
                      {ex.restSeconds ? ` · ${ex.restSeconds}s rest` : ''}
                    </span>
                  </li>
                ))}
              </ul>
            ) : (
              <p className="text-sm text-slate-400">No exercises.</p>
            )}
          </div>
        ))}
        {!plan.days?.length && (
          <div className="card text-sm text-slate-400 sm:col-span-2 lg:col-span-3">
            This plan has no workout days yet.
          </div>
        )}
      </div>

      {user?.role === 'user' && (
        <Link to={`/sessions/new?planId=${plan._id}`} className="btn-primary inline-block">
          Start workout
        </Link>
      )}

      {user?.role === 'trainer' && <PlanAssignments planId={plan._id} />}
    </div>
  );
}

function PlanAssignments({ planId }) {
  const qc = useQueryClient();
  const [clientId, setClientId] = useState('');

  const assigned = useQuery({
    queryKey: ['plan-assigned', planId],
    queryFn: () => api.get(`/trainer/plans/${planId}/clients`).then((r) => r.data),
  });
  const clients = useQuery({
    queryKey: ['trainer-clients'],
    queryFn: () => api.get('/trainer/clients').then((r) => r.data),
  });

  const assign = useMutation({
    mutationFn: (cid) => api.post(`/trainer/plans/${planId}/assign`, { clientId: cid }),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['plan-assigned', planId] });
      toast.success('Assigned');
      setClientId('');
    },
    onError: (err) => toast.error(err.response?.data?.detail || 'Failed to assign'),
  });
  const unassign = useMutation({
    mutationFn: (cid) => api.post(`/trainer/plans/${planId}/unassign`, { clientId: cid }),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['plan-assigned', planId] });
      toast.success('Unassigned');
    },
    onError: (err) => toast.error(err.response?.data?.detail || 'Failed to unassign'),
  });

  const assignedList = assigned.data?.items || [];
  const assignedIds = new Set(assignedList.map((c) => c._id));
  const available = (clients.data?.items || []).filter((c) => !assignedIds.has(c._id));

  return (
    <div className="card">
      <h3 className="font-semibold mb-1">Assigned to</h3>
      <p className="text-xs text-slate-400 mb-3">Your clients following this plan.</p>

      <ul className="divide-y divide-slate-100 mb-4">
        {assignedList.map((c) => (
          <li key={c._id} className="py-2 flex items-center justify-between text-sm">
            <span>
              {c.name} <span className="text-xs text-slate-400">{c.email}</span>
              {c.assignedAt && (
                <span className="text-xs text-slate-400"> · since {new Date(c.assignedAt).toLocaleDateString()}</span>
              )}
            </span>
            <button
              className="btn-secondary text-xs"
              disabled={unassign.isPending}
              onClick={() => unassign.mutate(c._id)}
            >
              Unassign
            </button>
          </li>
        ))}
        {!assignedList.length && (
          <li className="py-2 text-sm text-slate-400">Not assigned to any of your clients yet.</li>
        )}
      </ul>

      <div className="flex gap-2">
        <select
          className="input flex-1"
          value={clientId}
          onChange={(e) => setClientId(e.target.value)}
        >
          <option value="">Select a client…</option>
          {available.map((c) => (
            <option key={c._id} value={c._id}>
              {c.name} ({c.email})
            </option>
          ))}
        </select>
        <button
          className="btn-primary text-sm"
          disabled={!clientId || assign.isPending}
          onClick={() => assign.mutate(clientId)}
        >
          Assign
        </button>
      </div>
    </div>
  );
}
