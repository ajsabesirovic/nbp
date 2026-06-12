import { useQuery } from '@tanstack/react-query';
import { Link } from 'react-router-dom';
import { api } from '../api/client';
import { useAuth } from '../context/AuthContext';

function Stat({ label, value, hint }) {
  return (
    <div className="card">
      <div className="text-sm text-slate-500">{label}</div>
      <div className="text-3xl font-bold text-slate-900 mt-1">{value}</div>
      {hint && <div className="text-xs text-slate-400 mt-1">{hint}</div>}
    </div>
  );
}

export default function AdminDashboard() {
  const { user } = useAuth();
  const { data, isLoading } = useQuery({
    queryKey: ['admin-stats'],
    queryFn: () => api.get('/admin/stats').then((r) => r.data),
  });

  const fmt = (n) => (n ?? 0).toLocaleString();

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold">Welcome, {user?.name?.split(' ')[0]}.</h1>
        <p className="text-slate-500">Platform overview.</p>
      </div>

      {isLoading ? (
        <div className="text-slate-400">Loading…</div>
      ) : (
        <>
          <div>
            <h2 className="text-sm font-semibold text-slate-500 uppercase tracking-wide mb-2">
              People
            </h2>
            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
              <Stat
                label="Total users"
                value={fmt(data?.totalUsers)}
                hint={`${fmt(data?.newUsersLast7Days)} new in last 7 days`}
              />
              <Stat label="Regular users" value={fmt(data?.regularUsers)} />
              <Stat label="Trainers" value={fmt(data?.trainers)} />
              <Stat label="Admins" value={fmt(data?.admins)} />
            </div>
          </div>

          <div>
            <h2 className="text-sm font-semibold text-slate-500 uppercase tracking-wide mb-2">
              Activity
            </h2>
            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
              <Stat
                label="Workouts logged"
                value={fmt(data?.totalSessions)}
                hint="all-time, platform-wide"
              />
              <Stat
                label="Workouts this week"
                value={fmt(data?.sessionsLast7Days)}
                hint="last 7 days"
              />
              <Stat
                label="Active users"
                value={fmt(data?.activeUsersLast7Days)}
                hint="trained in last 7 days"
              />
              <Stat
                label="Plans"
                value={fmt(data?.totalPlans)}
                hint={`${fmt(data?.publishedPublicPlans)} published public`}
              />
            </div>
          </div>

          <div className="card">
            <h2 className="font-semibold mb-3">Manage</h2>
            <div className="flex gap-2 flex-wrap">
              <Link to="/admin" className="btn-secondary text-sm">
                Users
              </Link>
              <Link to="/trainer" className="btn-secondary text-sm">
                Trainers
              </Link>
              <Link to="/plans" className="btn-secondary text-sm">
                Plans
              </Link>
              <Link to="/system" className="btn-secondary text-sm">
                System logs
              </Link>
            </div>
          </div>
        </>
      )}
    </div>
  );
}
