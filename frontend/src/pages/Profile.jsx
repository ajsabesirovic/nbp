import { useEffect, useState } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Navigate } from 'react-router-dom';
import toast from 'react-hot-toast';
import { api } from '../api/client';
import { useAuth } from '../context/AuthContext';

export default function Profile() {
  const { user } = useAuth();
  if (user?.role === 'admin') return <Navigate to="/" replace />;
  if (user?.role === 'trainer') return <TrainerProfile />;
  return <UserProfile />;
}

function UserProfile() {
  const qc = useQueryClient();
  const { data, isLoading } = useQuery({
    queryKey: ['my-profile'],
    queryFn: () => api.get('/profile/me').then((r) => r.data),
  });

  const [form, setForm] = useState({ name: '', profile: {} });
  useEffect(() => {
    if (data?.user)
      setForm({
        name: data.user.name,
        profile: {
          gender: data.user.profile?.gender || '',
          dateOfBirth: data.user.profile?.dateOfBirth ? data.user.profile.dateOfBirth.slice(0, 10) : '',
          heightCm: data.user.profile?.heightCm || '',
          currentWeightKg: data.user.profile?.currentWeightKg || '',
          targetWeightKg: data.user.profile?.targetWeightKg || '',
          experience: data.user.profile?.experience || '',
          goal: data.user.profile?.goal || '',
        },
      });
  }, [data]);

  const save = useMutation({
    mutationFn: (p) => api.patch('/profile/me', p),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['my-profile'] });
      toast.success('Profile saved');
    },
  });

  if (isLoading) return <div>Loading...</div>;
  const numericOrNull = (v) => (v === '' ? undefined : Number(v));

  const submit = (e) => {
    e.preventDefault();
    save.mutate({
      name: form.name,
      profile: {
        gender: form.profile.gender || undefined,
        dateOfBirth: form.profile.dateOfBirth || undefined,
        heightCm: numericOrNull(form.profile.heightCm),
        currentWeightKg: numericOrNull(form.profile.currentWeightKg),
        targetWeightKg: numericOrNull(form.profile.targetWeightKg),
        experience: form.profile.experience || undefined,
        goal: form.profile.goal || undefined,
      },
    });
  };

  const upd = (k) => (e) => setForm({ ...form, profile: { ...form.profile, [k]: e.target.value } });

  return (
    <form onSubmit={submit} className="space-y-4 max-w-2xl">
      <h1 className="text-2xl font-bold">My profile</h1>
      <div className="card space-y-3">
        <div>
          <label className="label">Name</label>
          <input className="input" value={form.name} onChange={(e) => setForm({ ...form, name: e.target.value })} />
        </div>
        <div className="grid grid-cols-2 gap-3">
          <div>
            <label className="label">Gender</label>
            <select className="input" value={form.profile.gender} onChange={upd('gender')}>
              <option value="">—</option>
              <option value="male">male</option>
              <option value="female">female</option>
              <option value="other">other</option>
            </select>
          </div>
          <div>
            <label className="label">Experience</label>
            <select className="input" value={form.profile.experience} onChange={upd('experience')}>
              <option value="">—</option>
              <option>beginner</option>
              <option>intermediate</option>
              <option>advanced</option>
            </select>
          </div>
          <div>
            <label className="label">Date of birth</label>
            <input type="date" className="input" value={form.profile.dateOfBirth} onChange={upd('dateOfBirth')} />
          </div>
          <div>
            <label className="label">Height (cm)</label>
            <input type="number" className="input" value={form.profile.heightCm} onChange={upd('heightCm')} />
          </div>
          <div>
            <label className="label">Current weight (kg)</label>
            <input type="number" step="0.1" className="input" value={form.profile.currentWeightKg} onChange={upd('currentWeightKg')} />
          </div>
          <div>
            <label className="label">Target weight (kg)</label>
            <input type="number" step="0.1" className="input" value={form.profile.targetWeightKg} onChange={upd('targetWeightKg')} />
          </div>
          <div>
            <label className="label">Goal</label>
            <select className="input" value={form.profile.goal} onChange={upd('goal')}>
              <option value="">—</option>
              <option value="weight_loss">weight_loss</option>
              <option value="muscle_gain">muscle_gain</option>
              <option value="endurance">endurance</option>
              <option value="general_fitness">general_fitness</option>
            </select>
          </div>
        </div>
      </div>
      <button className="btn-primary" disabled={save.isPending}>
        Save
      </button>
    </form>
  );
}

function TrainerProfile() {
  const qc = useQueryClient();
  const { data, isLoading } = useQuery({
    queryKey: ['trainer-profile'],
    queryFn: () => api.get('/trainer/profile').then((r) => r.data),
  });

  const [form, setForm] = useState({ certifications: '', specialization: '', pricePerPlan: '', bio: '' });
  useEffect(() => {
    if (data)
      setForm({
        certifications: (data.certifications || []).join(', '),
        specialization: data.specialization || '',
        pricePerPlan: data.pricePerPlan ?? '',
        bio: data.bio || '',
      });
  }, [data]);

  const save = useMutation({
    mutationFn: (payload) => api.patch('/trainer/profile', payload),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['trainer-profile'] });
      toast.success('Profile saved');
    },
    onError: (err) => toast.error(err.response?.data?.detail || 'Save failed'),
  });

  if (isLoading) return <div>Loading...</div>;

  const submit = (e) => {
    e.preventDefault();
    save.mutate({
      certifications: form.certifications.split(',').map((s) => s.trim()).filter(Boolean),
      specialization: form.specialization || undefined,
      pricePerPlan: form.pricePerPlan === '' ? undefined : Number(form.pricePerPlan),
      bio: form.bio || undefined,
    });
  };

  return (
    <form onSubmit={submit} className="space-y-4 max-w-2xl">
      <h1 className="text-2xl font-bold">My trainer profile</h1>
      <div className="card space-y-3">
        <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
          <div>
            <label className="label">Specialization</label>
            <input
              className="input"
              placeholder="e.g. Strength & conditioning"
              value={form.specialization}
              onChange={(e) => setForm({ ...form, specialization: e.target.value })}
            />
          </div>
          <div>
            <label className="label">Price per plan</label>
            <input
              type="number"
              step="0.01"
              min="0"
              className="input"
              value={form.pricePerPlan}
              onChange={(e) => setForm({ ...form, pricePerPlan: e.target.value })}
            />
          </div>
        </div>
        <div>
          <label className="label">Certifications (comma-separated)</label>
          <input
            className="input"
            placeholder="e.g. NASM-CPT, CSCS"
            value={form.certifications}
            onChange={(e) => setForm({ ...form, certifications: e.target.value })}
          />
        </div>
        <div>
          <label className="label">Bio</label>
          <textarea
            className="input"
            rows={4}
            value={form.bio}
            onChange={(e) => setForm({ ...form, bio: e.target.value })}
          />
        </div>
      </div>
      <button className="btn-primary" disabled={save.isPending}>
        {save.isPending ? 'Saving...' : 'Save profile'}
      </button>
    </form>
  );
}
