import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import {
  Bar,
  BarChart,
  CartesianGrid,
  Cell,
  Legend,
  Line,
  LineChart,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from 'recharts';
import { api } from '../api/client';

const COLORS = ['#16a34a', '#0ea5e9', '#f59e0b', '#ef4444', '#8b5cf6', '#ec4899', '#14b8a6', '#f97316'];

export default function Progress() {
  const [exerciseId, setExerciseId] = useState('');

  const exercises = useQuery({
    queryKey: ['exercises-pick'],
    queryFn: () => api.get('/exercises?limit=200').then((r) => r.data),
  });
  const weekly = useQuery({
    queryKey: ['weekly'],
    queryFn: () => api.get('/progress/weekly').then((r) => r.data),
  });
  const muscle = useQuery({
    queryKey: ['muscle'],
    queryFn: () => api.get('/progress/muscle-balance').then((r) => r.data),
  });
  const measurements = useQuery({
    queryKey: ['measurements'],
    queryFn: () => api.get('/progress/measurements').then((r) => r.data),
  });
  const progression = useQuery({
    queryKey: ['progression', exerciseId],
    queryFn: () => api.get(`/progress/progression/${exerciseId}`).then((r) => r.data),
    enabled: !!exerciseId,
  });

  const weeklyData = (Array.isArray(weekly.data) ? weekly.data : []).map((w) => ({
    label: `${w.year}-W${w.week}`,
    volume: Math.round(w.totalVolumeKg),
    workouts: w.sessionCount ?? w.workouts ?? 0,
  }));

  const muscleData = Array.isArray(muscle.data) ? muscle.data : [];
  const progressionData = (Array.isArray(progression.data) ? progression.data : []).map((p) => ({
    date: new Date(p.date).toLocaleDateString(),
    maxWeightKg: p.maxWeightKg,
    oneRepMax: p.oneRepMax,
  }));
  const measurementData = (measurements.data?.items || [])
    .filter((m) => m.weightKg)
    .map((m) => ({ date: new Date(m.recordedAt).toLocaleDateString(), weight: m.weightKg }))
    .reverse();

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold">Progress</h1>

      <div className="card">
        <h2 className="font-semibold mb-3">Weekly training volume</h2>
        <div className="h-64">
          <ResponsiveContainer width="100%" height="100%">
            <BarChart data={weeklyData}>
              <CartesianGrid strokeDasharray="3 3" stroke="#e2e8f0" />
              <XAxis dataKey="label" />
              <YAxis />
              <Tooltip cursor={false} />
              <Legend />
              <Bar dataKey="volume" fill="#16a34a" name="Volume (kg)" activeBar={false} />
            </BarChart>
          </ResponsiveContainer>
        </div>
      </div>

      <div className="card">
        <h2 className="font-semibold mb-3">Workout frequency</h2>
        <div className="h-56">
          <ResponsiveContainer width="100%" height="100%">
            <BarChart data={weeklyData}>
              <CartesianGrid strokeDasharray="3 3" stroke="#e2e8f0" />
              <XAxis dataKey="label" />
              <YAxis allowDecimals={false} />
              <Tooltip cursor={false} />
              <Legend />
              <Bar dataKey="workouts" fill="#0ea5e9" name="Workouts / week" activeBar={false} />
            </BarChart>
          </ResponsiveContainer>
        </div>
      </div>

      <div className="card">
        <div className="flex justify-between items-center mb-3">
          <h2 className="font-semibold">Exercise progression</h2>
          <select className="input max-w-xs" value={exerciseId} onChange={(e) => setExerciseId(e.target.value)}>
            <option value="">Pick exercise...</option>
            {exercises.data?.items?.map((e) => (
              <option key={e._id} value={e._id}>
                {e.name}
              </option>
            ))}
          </select>
        </div>
        <div className="h-64">
          <ResponsiveContainer width="100%" height="100%">
            <LineChart data={progressionData}>
              <CartesianGrid strokeDasharray="3 3" stroke="#e2e8f0" />
              <XAxis dataKey="date" />
              <YAxis />
              <Tooltip />
              <Legend />
              <Line type="monotone" dataKey="maxWeightKg" stroke="#16a34a" name="Max weight (kg)" />
              <Line type="monotone" dataKey="oneRepMax" stroke="#0ea5e9" name="Estimated 1RM" />
            </LineChart>
          </ResponsiveContainer>
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-4">
        <div className="card">
          <h2 className="font-semibold mb-3">Muscle group balance</h2>
          <div className="h-64">
            <ResponsiveContainer width="100%" height="100%">
              <BarChart data={muscleData} layout="vertical">
                <CartesianGrid strokeDasharray="3 3" stroke="#e2e8f0" />
                <XAxis type="number" />
                <YAxis type="category" dataKey="muscle" width={90} />
                <Tooltip cursor={false} />
                <Bar dataKey="volumeKg" name="Volume (kg)" activeBar={false}>
                  {muscleData.map((_, i) => (
                    <Cell key={i} fill={COLORS[i % COLORS.length]} />
                  ))}
                </Bar>
              </BarChart>
            </ResponsiveContainer>
          </div>
        </div>

        <div className="card">
          <h2 className="font-semibold mb-3">Body weight over time</h2>
          <div className="h-64">
            <ResponsiveContainer width="100%" height="100%">
              <LineChart data={measurementData}>
                <CartesianGrid strokeDasharray="3 3" stroke="#e2e8f0" />
                <XAxis dataKey="date" />
                <YAxis domain={['dataMin - 2', 'dataMax + 2']} />
                <Tooltip />
                <Line type="monotone" dataKey="weight" stroke="#16a34a" name="Weight (kg)" />
              </LineChart>
            </ResponsiveContainer>
          </div>
        </div>
      </div>

    </div>
  );
}
