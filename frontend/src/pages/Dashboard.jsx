import { useQuery } from '@tanstack/react-query';
import { Link, Navigate } from 'react-router-dom';
import {
  LineChart,
  Line,
  XAxis,
  YAxis,
  Tooltip,
  CartesianGrid,
  ResponsiveContainer,
} from 'recharts';
import { api } from '../api/client';
import { useAuth } from '../context/AuthContext';
import AdminDashboard from './AdminDashboard';

function getISOWeek(date) {
  const d = new Date(Date.UTC(date.getFullYear(), date.getMonth(), date.getDate()));
  const dayNum = d.getUTCDay() || 7;
  d.setUTCDate(d.getUTCDate() + 4 - dayNum);
  const yearStart = new Date(Date.UTC(d.getUTCFullYear(), 0, 1));
  const week = Math.ceil(((d - yearStart) / 86400000 + 1) / 7);
  return { year: d.getUTCFullYear(), week };
}

function Stat({ label, value, hint }) {
  return (
    <div className="card">
      <div className="text-sm text-slate-500">{label}</div>
      <div className="text-3xl font-bold text-slate-900 mt-1">{value}</div>
      {hint && <div className="text-xs text-slate-400 mt-1">{hint}</div>}
    </div>
  );
}

export default function Dashboard() {
  const { user } = useAuth();

  if (user?.role === 'admin') return <AdminDashboard />;

  if (user?.role === 'trainer') return <Navigate to="/trainer" replace />;
  return <UserDashboard />;
}

function UserDashboard() {
  const { user } = useAuth();

  const streak = useQuery({
    queryKey: ['streak'],
    queryFn: () => api.get('/progress/streak').then((r) => r.data),
  });
  const weekly = useQuery({
    queryKey: ['weekly'],
    queryFn: () => api.get('/progress/weekly').then((r) => r.data),
  });
  const prs = useQuery({
    queryKey: ['prs'],
    queryFn: () => api.get('/progress/prs').then((r) => r.data),
  });
  const recentSessions = useQuery({
    queryKey: ['recentSessions'],
    queryFn: () => api.get('/sessions?limit=5').then((r) => r.data),
  });

  const weeklyItems = Array.isArray(weekly.data) ? weekly.data : [];
  const chartData = weeklyItems.map((w) => ({
    label: `W${w.week}`,
    volume: Math.round(w.totalVolumeKg),
    workouts: w.sessionCount,
  }));

  const today = getISOWeek(new Date());
  const thisWeek = weeklyItems.find((w) => w.year === today.year && w.week === today.week);
  const thisWeekVolume = thisWeek ? Math.round(thisWeek.totalVolumeKg) : 0;
  const thisWeekWorkouts = thisWeek?.sessionCount ?? 0;

  const prList = Array.isArray(prs.data) ? prs.data : [];
  const currentStreak = streak.data?.currentStreak ?? 0;

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold">Welcome back, {user?.name?.split(' ')[0]}.</h1>
          <p className="text-slate-500">Here's a snapshot of your training.</p>
        </div>
        <Link to="/sessions/new" className="btn-primary">
          Start workout
        </Link>
      </div>

      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
        <Stat
          label="Current streak"
          value={currentStreak ? `${currentStreak} 🔥` : '0'}
          hint={currentStreak ? 'consecutive days' : 'log a workout to start'}
        />
        <Stat label="Workout days" value={streak.data?.totalWorkoutDays ?? 0} hint="all-time" />
        <Stat label="Personal records" value={prList.length} />
        <Stat
          label="This week"
          value={`${thisWeekVolume.toLocaleString()} kg`}
          hint={`${thisWeekWorkouts} workout${thisWeekWorkouts === 1 ? '' : 's'}`}
        />
      </div>

      <div className="card">
        <h2 className="font-semibold mb-3">Weekly volume</h2>
        {weeklyItems.length === 0 ? (
          <p className="text-sm text-slate-400">No data yet.</p>
        ) : (
          <div className="h-64">
            <ResponsiveContainer width="100%" height="100%">
              <LineChart data={chartData}>
                <CartesianGrid strokeDasharray="3 3" stroke="#e2e8f0" />
                <XAxis dataKey="label" />
                <YAxis />
                <Tooltip formatter={(v) => [`${v} kg`, 'Volume']} />
                <Line type="monotone" dataKey="volume" stroke="#16a34a" strokeWidth={2} dot={{ r: 3 }} />
              </LineChart>
            </ResponsiveContainer>
          </div>
        )}
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-4">
        <div className="card">
          <h2 className="font-semibold mb-3">Recent sessions</h2>
          <ul className="divide-y divide-slate-200">
            {(recentSessions.data?.items || []).map((s) => (
              <li key={s.id} className="py-2 flex justify-between text-sm">
                <span>{new Date(s.startedAt).toLocaleDateString()}</span>
                <span className="text-slate-500">
                  {s.exercises?.length ?? 0} exercises · {Math.round(s.totalVolumeKg)} kg
                </span>
              </li>
            ))}
            {!recentSessions.data?.items?.length && (
              <li className="py-2 text-sm text-slate-400">No sessions yet.</li>
            )}
          </ul>
        </div>
        <div className="card">
          <h2 className="font-semibold mb-3">Personal records</h2>
          <ul className="divide-y divide-slate-200">
            {prList.slice(0, 8).map((p) => {
              const type = (p.type || '1rm').toLowerCase();
              const detail =
                type === '5rm'
                  ? `${p.weightKg} kg × ${p.reps} reps`
                  : type === 'reps'
                  ? `${p.reps} reps @ ${p.weightKg} kg`
                  : `${p.weightKg} kg × ${p.reps} — ${p.oneRepMax.toFixed(1)} kg 1RM`;
              return (
                <li key={p.id} className="py-2 flex justify-between items-center text-sm">
                  <span>
                    {p.exerciseName || 'Exercise'}{' '}
                    <span className="text-xs bg-slate-100 text-slate-600 rounded px-1.5 py-0.5 ml-1 uppercase">
                      {type}
                    </span>
                  </span>
                  <span className="text-slate-700 font-medium">{detail}</span>
                </li>
              );
            })}
            {prList.length === 0 && (
              <li className="py-2 text-sm text-slate-400">No records yet.</li>
            )}
          </ul>
        </div>
      </div>
    </div>
  );
}
