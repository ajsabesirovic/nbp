import { useState } from 'react';
import { Link } from 'react-router-dom';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import toast from 'react-hot-toast';
import { Pencil, Trash2 } from 'lucide-react';
import { api } from '../api/client';
import { useAuth } from '../context/AuthContext';
import IconButton from '../components/IconButton';

const canEdit = (plan, user) => {
  if (!user) return false;
  if (user.role === 'admin') return true;
  const uid = String(user._id ?? user.id ?? '');
  return uid && String(plan.authorId) === uid;
};

export default function Plans() {
  const { user } = useAuth();

  if (user?.role === 'admin') return <AdminPlans />;

  if (user?.role === 'trainer') return <TrainerPlans />;
  return <UserPlans />;
}

function TrainerPlans() {
  const { user } = useAuth();
  const qc = useQueryClient();
  const [tab, setTab] = useState('public');

  const params = tab === 'public' ? { visibility: 'public' } : { mine: true };
  const { data, isLoading } = useQuery({
    queryKey: ['trainer-plans', tab],
    queryFn: () => api.get('/plans', { params }).then((r) => r.data),
  });
  const del = useMutation({
    mutationFn: (id) => api.delete(`/plans/${id}`),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['trainer-plans'] });
      toast.success('Deleted');
    },
  });

  const TABS = [
    { id: 'public', label: 'Public library' },
    { id: 'mine', label: 'My plans' },
  ];

  return (
    <div className="space-y-4">
      <div className="flex justify-between items-center">
        <h1 className="text-2xl font-bold">Workout plans</h1>
        <Link to="/plans/new" className="btn-primary">
          + New plan
        </Link>
      </div>

      <div className="flex gap-2">
        {TABS.map((t) => (
          <button
            key={t.id}
            onClick={() => setTab(t.id)}
            className={`px-3 py-1 rounded-md text-sm ${
              tab === t.id ? 'bg-brand-600 text-white' : 'bg-slate-200 border border-slate-300'
            }`}
          >
            {t.label}
          </button>
        ))}
      </div>

      {isLoading ? (
        <div>Loading...</div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          {data?.items?.map((p) => (
            <div key={p._id} className="card">
              <div className="flex items-start justify-between gap-2">
                <h3 className="font-semibold">{p.name}</h3>
                <span className="badge bg-brand-50 text-brand-700">{p.visibility}</span>
              </div>
              <p className="text-sm text-slate-500 mt-1 line-clamp-2">{p.description}</p>
              <div className="text-xs text-slate-400 mt-2">
                {p.durationWeeks ? `${p.durationWeeks} wk · ` : ''}
                {p.daysPerWeek ? `${p.daysPerWeek}×/wk · ` : ''}
                {p.level} · {p.goal}
              </div>
              <div className="text-xs text-slate-500 mt-2">
                {p.days?.length || 0} workout days · by {p.authorName}
              </div>
              <div className="flex gap-2 mt-3 flex-wrap">
                <Link to={`/plans/${p._id}`} className="btn-primary text-xs">
                  View
                </Link>
                {canEdit(p, user) && (
                  <IconButton icon={Pencil} label="Edit plan" variant="default" to={`/plans/${p._id}/edit`} />
                )}
                {canEdit(p, user) && (
                  <IconButton
                    icon={Trash2}
                    label="Delete plan"
                    variant="danger"
                    onClick={() => confirm('Delete?') && del.mutate(p._id)}
                  />
                )}
              </div>
            </div>
          ))}
          {!data?.items?.length && <div className="text-slate-400">No plans found.</div>}
        </div>
      )}
    </div>
  );
}

function UserPlans() {
  const { user, setActivePlan } = useAuth();
  const qc = useQueryClient();
  const [tab, setTab] = useState('public');
  const activePlanId = user?.activePlanId || '';

  const activate = useMutation({
    mutationFn: (id) => setActivePlan(id),
    onSuccess: (_d, id) =>
      toast.success(id ? 'Set as your active plan' : 'Active plan cleared'),
    onError: (err) => toast.error(err.response?.data?.error || 'Failed'),
  });

  const params =
    tab === 'public' ? { visibility: 'public' } : tab === 'mine' ? { mine: true } : { assignedToMe: true };

  const { data, isLoading } = useQuery({
    queryKey: ['plans', tab],
    queryFn: () => api.get('/plans', { params }).then((r) => r.data),
  });

  const del = useMutation({
    mutationFn: (id) => api.delete(`/plans/${id}`),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['plans'] });
      toast.success('Deleted');
    },
  });

  const TABS = [
    { id: 'public', label: 'Public library' },
    { id: 'mine', label: 'My plans' },
    { id: 'assigned', label: 'Assigned to me' },
  ];

  return (
    <div className="space-y-4">
      <div className="flex justify-between items-center">
        <h1 className="text-2xl font-bold">Workout plans</h1>
        <Link to="/plans/new" className="btn-primary">
          + New plan
        </Link>
      </div>

      <div className="flex gap-2">
        {TABS.map((t) => (
          <button
            key={t.id}
            onClick={() => setTab(t.id)}
            className={`px-3 py-1 rounded-md text-sm ${
              tab === t.id ? 'bg-brand-600 text-white' : 'bg-slate-200 border border-slate-300'
            }`}
          >
            {t.label}
          </button>
        ))}
      </div>

      {isLoading ? (
        <div>Loading...</div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          {data?.items?.map((p) => (
            <div key={p._id} className="card">
              <div className="flex items-start justify-between gap-2">
                <h3 className="font-semibold">
                  {p.name}
                  {p._id === activePlanId && (
                    <span className="badge bg-green-500/15 text-green-300 ml-2">active</span>
                  )}
                </h3>
                <span className="badge bg-brand-50 text-brand-700">{p.visibility}</span>
              </div>
              <p className="text-sm text-slate-500 mt-1 line-clamp-2">{p.description}</p>
              <div className="text-xs text-slate-400 mt-2">
                {p.durationWeeks ? `${p.durationWeeks} wk · ` : ''}
                {p.daysPerWeek ? `${p.daysPerWeek}×/wk · ` : ''}
                {p.level} · {p.goal}
              </div>
              <div className="text-xs text-slate-500 mt-2">{p.days?.length || 0} workout days</div>
              <div className="flex gap-2 mt-3 flex-wrap">
                <Link to={`/sessions/new?planId=${p._id}`} className="btn-primary text-xs">
                  Start workout
                </Link>
                {p._id === activePlanId ? (
                  <button
                    className="btn-secondary text-xs"
                    onClick={() => activate.mutate(null)}
                    disabled={activate.isPending}
                  >
                    Unset active
                  </button>
                ) : (
                  <button
                    className="btn-secondary text-xs"
                    onClick={() => activate.mutate(p._id)}
                    disabled={activate.isPending}
                  >
                    Set as active
                  </button>
                )}
                {canEdit(p, user) && (
                  <IconButton
                    icon={Pencil}
                    label="Edit plan"
                    variant="default"
                    to={`/plans/${p._id}/edit`}
                  />
                )}
                {canEdit(p, user) && (
                  <IconButton
                    icon={Trash2}
                    label="Delete plan"
                    variant="danger"
                    onClick={() => confirm('Delete?') && del.mutate(p._id)}
                  />
                )}
              </div>
            </div>
          ))}
          {!data?.items?.length && <div className="text-slate-400">No plans found.</div>}
        </div>
      )}
    </div>
  );
}

function AdminPlans() {
  const qc = useQueryClient();
  const [tab, setTab] = useState('all');

  const TABS = [
    { id: 'all', label: 'All plans', params: {} },
    { id: 'trainer', label: 'Trainer-authored', params: { authorRole: 'trainer' } },
    { id: 'user', label: 'User-authored', params: { authorRole: 'user' } },
  ];
  const active = TABS.find((t) => t.id === tab);

  const { data, isLoading } = useQuery({
    queryKey: ['admin-plans', tab],
    queryFn: () => api.get('/admin/plans', { params: active.params }).then((r) => r.data),
  });

  const moderate = useMutation({
    mutationFn: ({ id, status }) => api.patch(`/admin/plans/${id}/status`, { status }),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['admin-plans'] });
      toast.success('Status updated');
    },
    onError: (err) => toast.error(err.response?.data?.error || 'Failed'),
  });

  const del = useMutation({
    mutationFn: (id) => api.delete(`/plans/${id}`),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['admin-plans'] });
      toast.success('Deleted');
    },
  });

  const statusColor = (s) =>
    s === 'published'
      ? 'bg-green-500/15 text-green-300'
      : s === 'archived'
      ? 'bg-red-500/15 text-red-300'
      : 'bg-amber-500/15 text-amber-300';

  return (
    <div className="space-y-4">
      <div>
        <h1 className="text-2xl font-bold">Workout plans</h1>
        <p className="text-sm text-slate-500">Browse and moderate every plan on the platform.</p>
      </div>

      <div className="flex gap-2 flex-wrap">
        {TABS.map((t) => (
          <button
            key={t.id}
            onClick={() => setTab(t.id)}
            className={`px-3 py-1 rounded-md text-sm ${
              tab === t.id ? 'bg-brand-600 text-white' : 'bg-slate-200 border border-slate-300'
            }`}
          >
            {t.label}
          </button>
        ))}
      </div>

      {isLoading ? (
        <div>Loading...</div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          {data?.items?.map((p) => (
            <div key={p._id} className="card">
              <div className="flex items-start justify-between gap-2">
                <h3 className="font-semibold">{p.name}</h3>
                <div className="flex gap-1 shrink-0">
                  <span className="badge bg-brand-50 text-brand-700">{p.visibility}</span>
                  <span className={`badge ${statusColor(p.status)}`}>{p.status}</span>
                </div>
              </div>
              <p className="text-sm text-slate-500 mt-1 line-clamp-2">{p.description}</p>
              <div className="text-xs text-slate-400 mt-2">
                by {p.authorName} · {p.level} · {p.goal}
              </div>
              <div className="text-xs text-slate-500 mt-1">{p.days?.length || 0} workout days</div>
              <div className="flex gap-2 mt-3 flex-wrap">
                <button
                  className="btn-secondary text-xs"
                  disabled={moderate.isPending || p.status === 'published'}
                  onClick={() => moderate.mutate({ id: p._id, status: 'published' })}
                >
                  Publish
                </button>
                <button
                  className="btn-secondary text-xs"
                  disabled={moderate.isPending || p.status === 'draft'}
                  onClick={() => moderate.mutate({ id: p._id, status: 'draft' })}
                >
                  Unpublish
                </button>
                <button
                  className="btn-secondary text-xs"
                  disabled={moderate.isPending || p.status === 'archived'}
                  onClick={() => moderate.mutate({ id: p._id, status: 'archived' })}
                >
                  Archive
                </button>
                <IconButton
                  icon={Pencil}
                  label="Edit plan"
                  variant="default"
                  to={`/plans/${p._id}/edit`}
                />
                <IconButton
                  icon={Trash2}
                  label="Delete plan"
                  variant="danger"
                  onClick={() => confirm('Delete this plan?') && del.mutate(p._id)}
                />
              </div>
            </div>
          ))}
          {!data?.items?.length && <div className="text-slate-400">No plans found.</div>}
        </div>
      )}
    </div>
  );
}
