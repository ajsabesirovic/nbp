import { useEffect, useRef, useState } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import toast from 'react-hot-toast';
import { Pencil, Trash2 } from 'lucide-react';
import { api } from '../api/client';
import { useAuth } from '../context/AuthContext';
import IconButton from '../components/IconButton';

const TYPES = ['weighted', 'bodyweight', 'endurance', 'isometric'];

const CATEGORIES = ['strength', 'cardio', 'mobility', 'flexibility'];

function useDebounce(value, delay = 350) {
  const [debounced, setDebounced] = useState(value);
  useEffect(() => {
    const id = setTimeout(() => setDebounced(value), delay);
    return () => clearTimeout(id);
  }, [value, delay]);
  return debounced;
}

export default function Exercises() {
  const { user } = useAuth();
  const qc = useQueryClient();
  const [search, setSearch] = useState('');
  const [type, setType] = useState('');
  const [showCreate, setShowCreate] = useState(false);
  const [editing, setEditing] = useState(null);

  const debouncedSearch = useDebounce(search);

  const { data, isLoading } = useQuery({
    queryKey: ['exercises', { search: debouncedSearch, type }],
    queryFn: () =>
      api.get('/exercises', { params: { search: debouncedSearch || undefined, type: type || undefined, limit: 100 } })
        .then((r) => r.data),
  });

  const del = useMutation({
    mutationFn: (id) => api.delete(`/exercises/${id}`),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['exercises'] });
      toast.success('Deleted');
    },
  });

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold">Exercise library</h1>
        {user?.role === 'admin' && (
          <button className="btn-primary" onClick={() => setShowCreate(true)}>
            + New exercise
          </button>
        )}
      </div>

      <div className="card flex gap-3 flex-wrap">
        <input
          className="input max-w-xs"
          placeholder="Search by name..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
        />
        <select className="input max-w-xs" value={type} onChange={(e) => setType(e.target.value)}>
          <option value="">All types</option>
          {TYPES.map((t) => (
            <option key={t}>{t}</option>
          ))}
        </select>
      </div>

      {isLoading ? (
        <div className="text-slate-500">Loading...</div>
      ) : (
        <>
          <p className="text-sm text-slate-400">{data?.total ?? 0} exercises</p>
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
            {data?.items?.map((ex) => (
              <div key={ex.id} className="card">
                <div className="flex items-start justify-between gap-2">
                  <h3 className="font-semibold text-slate-900">{ex.name}</h3>
                  <span className="badge bg-brand-50 text-brand-700">{ex.type}</span>
                </div>
                <div className="text-sm mt-2">
                  <span className="text-slate-500">Primary:</span>{' '}
                  {ex.primaryMuscles?.join(', ') || '—'}
                </div>
                {ex.secondaryMuscles?.length > 0 && (
                  <div className="text-sm">
                    <span className="text-slate-500">Secondary:</span>{' '}
                    {ex.secondaryMuscles.join(', ')}
                  </div>
                )}
                {(ex.category || ex.equipment || ex.difficulty) && (
                  <div className="text-xs text-slate-500 mt-1 flex flex-wrap gap-x-2">
                    {ex.category && <span>Category: {ex.category}</span>}
                    {ex.equipment && <span>· Equipment: {ex.equipment}</span>}
                    {ex.difficulty && <span>· Difficulty: {ex.difficulty}/5</span>}
                  </div>
                )}
                {ex.description && (
                  <div className="text-xs text-slate-400 mt-1 line-clamp-2">{ex.description}</div>
                )}
                {user?.role === 'admin' && (
                  <div className="flex gap-1 mt-3">
                    <IconButton
                      icon={Pencil}
                      label="Edit exercise"
                      variant="default"
                      onClick={() => setEditing(ex)}
                    />
                    <IconButton
                      icon={Trash2}
                      label="Delete exercise"
                      variant="danger"
                      onClick={() => confirm('Delete?') && del.mutate(ex.id)}
                    />
                  </div>
                )}
              </div>
            ))}
          </div>
        </>
      )}

      {showCreate && <CreateExercise onClose={() => setShowCreate(false)} />}
      {editing && <EditExercise exercise={editing} onClose={() => setEditing(null)} />}
    </div>
  );
}

function EditExercise({ exercise, onClose }) {
  const qc = useQueryClient();
  const [form, setForm] = useState({
    name: exercise.name || '',
    type: exercise.type || 'weighted',
    primaryMuscles: (exercise.primaryMuscles || []).join(', '),
    secondaryMuscles: (exercise.secondaryMuscles || []).join(', '),
    category: exercise.category || '',
    equipment: exercise.equipment || '',
    difficulty: exercise.difficulty ?? '',
    description: exercise.description || '',
    instructions: exercise.instructions || '',
    imageUrl: exercise.imageUrl || '',
    videoUrl: exercise.videoUrl || '',
  });

  const update = useMutation({
    mutationFn: (payload) => api.patch(`/exercises/${exercise.id}`, payload),
    onSuccess: () => {
      toast.success('Exercise updated');
      qc.invalidateQueries({ queryKey: ['exercises'] });
      qc.invalidateQueries({ queryKey: ['exercises-pick'] });
      onClose();
    },
    onError: (err) => toast.error(err.response?.data?.detail || 'Failed'),
  });

  const upd = (k) => (e) => setForm({ ...form, [k]: e.target.value });

  const submit = (e) => {
    e.preventDefault();
    update.mutate({
      name: form.name,
      type: form.type,
      primaryMuscles: form.primaryMuscles.split(',').map((s) => s.trim()).filter(Boolean),
      secondaryMuscles: form.secondaryMuscles.split(',').map((s) => s.trim()).filter(Boolean),
      category: form.category || undefined,
      equipment: form.equipment || undefined,
      difficulty: form.difficulty === '' ? undefined : Number(form.difficulty),
      description: form.description || undefined,
      instructions: form.instructions || undefined,
      imageUrl: form.imageUrl || undefined,
      videoUrl: form.videoUrl || undefined,
    });
  };

  return (
    <div className="fixed inset-0 bg-black/40 flex items-center justify-center z-20 p-4">
      <form onSubmit={submit} className="card w-full max-w-lg max-h-[90vh] overflow-auto">
        <h2 className="text-lg font-bold mb-4">Edit exercise</h2>
        <div className="space-y-3">
          <div>
            <label className="label">Name</label>
            <input className="input" value={form.name} onChange={upd('name')} required />
          </div>
          <div>
            <label className="label">Type</label>
            <select className="input" value={form.type} onChange={upd('type')}>
              {TYPES.map((t) => <option key={t}>{t}</option>)}
            </select>
          </div>
          <div>
            <label className="label">Primary muscles (comma-separated)</label>
            <input className="input" value={form.primaryMuscles} onChange={upd('primaryMuscles')} required />
          </div>
          <div>
            <label className="label">Secondary muscles (comma-separated)</label>
            <input className="input" value={form.secondaryMuscles} onChange={upd('secondaryMuscles')} />
          </div>
          <div className="grid grid-cols-2 gap-3">
            <div>
              <label className="label">Category</label>
              <select className="input" value={form.category} onChange={upd('category')}>
                <option value="">—</option>
                {CATEGORIES.map((c) => <option key={c}>{c}</option>)}
              </select>
            </div>
            <div>
              <label className="label">Difficulty (1–5)</label>
              <input type="number" min="1" max="5" className="input" value={form.difficulty} onChange={upd('difficulty')} />
            </div>
          </div>
          <div>
            <label className="label">Equipment</label>
            <input className="input" placeholder="e.g. barbell, dumbbell, none" value={form.equipment} onChange={upd('equipment')} />
          </div>
          <div className="grid grid-cols-2 gap-3">
            <div>
              <label className="label">Image URL</label>
              <input className="input" value={form.imageUrl} onChange={upd('imageUrl')} />
            </div>
            <div>
              <label className="label">Video URL</label>
              <input className="input" value={form.videoUrl} onChange={upd('videoUrl')} />
            </div>
          </div>
          <div>
            <label className="label">Description</label>
            <textarea className="input" rows="2" value={form.description} onChange={upd('description')} />
          </div>
          <div>
            <label className="label">Instructions</label>
            <textarea className="input" rows="3" value={form.instructions} onChange={upd('instructions')} />
          </div>
        </div>
        <div className="flex gap-2 justify-end mt-4">
          <button type="button" className="btn-secondary" onClick={onClose}>Cancel</button>
          <button className="btn-primary" disabled={update.isPending}>
            {update.isPending ? 'Saving...' : 'Save changes'}
          </button>
        </div>
      </form>
    </div>
  );
}

function CreateExercise({ onClose }) {
  const qc = useQueryClient();
  const [form, setForm] = useState({
    name: '',
    type: 'weighted',
    primaryMuscles: '',
    secondaryMuscles: '',
    category: '',
    equipment: '',
    difficulty: '',
    description: '',
    instructions: '',
    imageUrl: '',
    videoUrl: '',
  });

  const create = useMutation({
    mutationFn: (payload) => api.post('/exercises', payload),
    onSuccess: () => {
      toast.success('Exercise created');
      qc.invalidateQueries({ queryKey: ['exercises'] });
      onClose();
    },
    onError: (err) => toast.error(err.response?.data?.detail || 'Failed'),
  });

  const upd = (k) => (e) => setForm({ ...form, [k]: e.target.value });

  const submit = (e) => {
    e.preventDefault();
    create.mutate({
      name: form.name,
      type: form.type,
      primaryMuscles: form.primaryMuscles.split(',').map((s) => s.trim()).filter(Boolean),
      secondaryMuscles: form.secondaryMuscles.split(',').map((s) => s.trim()).filter(Boolean),
      category: form.category || undefined,
      equipment: form.equipment || undefined,
      difficulty: form.difficulty === '' ? undefined : Number(form.difficulty),
      description: form.description || undefined,
      instructions: form.instructions || undefined,
      imageUrl: form.imageUrl || undefined,
      videoUrl: form.videoUrl || undefined,
    });
  };

  return (
    <div className="fixed inset-0 bg-black/40 flex items-center justify-center z-20 p-4">
      <form onSubmit={submit} className="card w-full max-w-lg max-h-[90vh] overflow-auto">
        <h2 className="text-lg font-bold mb-4">New exercise</h2>
        <div className="space-y-3">
          <div>
            <label className="label">Name</label>
            <input className="input" value={form.name} onChange={upd('name')} required />
          </div>
          <div>
            <label className="label">Type</label>
            <select className="input" value={form.type} onChange={upd('type')}>
              {TYPES.map((t) => <option key={t}>{t}</option>)}
            </select>
          </div>
          <div>
            <label className="label">Primary muscles (comma-separated)</label>
            <input className="input" value={form.primaryMuscles} onChange={upd('primaryMuscles')} required />
          </div>
          <div>
            <label className="label">Secondary muscles (comma-separated)</label>
            <input className="input" value={form.secondaryMuscles} onChange={upd('secondaryMuscles')} />
          </div>
          <div className="grid grid-cols-2 gap-3">
            <div>
              <label className="label">Category</label>
              <select className="input" value={form.category} onChange={upd('category')}>
                <option value="">—</option>
                {CATEGORIES.map((c) => <option key={c}>{c}</option>)}
              </select>
            </div>
            <div>
              <label className="label">Difficulty (1–5)</label>
              <input type="number" min="1" max="5" className="input" value={form.difficulty} onChange={upd('difficulty')} />
            </div>
          </div>
          <div>
            <label className="label">Equipment</label>
            <input className="input" placeholder="e.g. barbell, dumbbell, none" value={form.equipment} onChange={upd('equipment')} />
          </div>
          <div className="grid grid-cols-2 gap-3">
            <div>
              <label className="label">Image URL</label>
              <input className="input" value={form.imageUrl} onChange={upd('imageUrl')} />
            </div>
            <div>
              <label className="label">Video URL</label>
              <input className="input" value={form.videoUrl} onChange={upd('videoUrl')} />
            </div>
          </div>
          <div>
            <label className="label">Description</label>
            <textarea className="input" rows="2" value={form.description} onChange={upd('description')} />
          </div>
          <div>
            <label className="label">Instructions</label>
            <textarea className="input" rows="3" value={form.instructions} onChange={upd('instructions')} />
          </div>
        </div>
        <div className="flex gap-2 justify-end mt-4">
          <button type="button" className="btn-secondary" onClick={onClose}>Cancel</button>
          <button className="btn-primary" disabled={create.isPending}>Create</button>
        </div>
      </form>
    </div>
  );
}
