import { useState } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import toast from 'react-hot-toast';
import { Trash2 } from 'lucide-react';
import { api, assetUrl } from '../api/client';
import IconButton from '../components/IconButton';

export default function Photos() {
  const qc = useQueryClient();
  const [file, setFile] = useState(null);
  const [note, setNote] = useState('');
  const [takenAt, setTakenAt] = useState('');

  const [resetKey, setResetKey] = useState(0);

  const { data, isLoading } = useQuery({
    queryKey: ['photos'],
    queryFn: () => api.get('/photos').then((r) => r.data),
  });

  const upload = useMutation({
    mutationFn: (form) =>
      api.post('/photos', form, { headers: { 'Content-Type': 'multipart/form-data' } }),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['photos'] });
      toast.success('Photo uploaded');
      setFile(null);
      setNote('');
      setTakenAt('');
      setResetKey((k) => k + 1);
    },
    onError: (err) => toast.error(err.response?.data?.error || 'Upload failed'),
  });

  const del = useMutation({
    mutationFn: (id) => api.delete(`/photos/${id}`),
    onSuccess: () => qc.invalidateQueries({ queryKey: ['photos'] }),
  });

  const submit = (e) => {
    e.preventDefault();
    if (!file) return;
    const form = new FormData();
    form.append('file', file);
    if (note) form.append('note', note);
    if (takenAt) form.append('takenAt', takenAt);
    upload.mutate(form);
  };

  return (
    <div className="space-y-4">
      <h1 className="text-2xl font-bold">Progress photos</h1>

      <form onSubmit={submit} className="card space-y-3">
        <div className="grid grid-cols-1 md:grid-cols-3 gap-3">
          <div>
            <label className="label">Photo</label>
            <input
              key={resetKey}
              type="file"
              accept="image/jpeg,image/png,image/webp"
              className="input"
              onChange={(e) => setFile(e.target.files?.[0] || null)}
            />
          </div>
          <div>
            <label className="label">Taken</label>
            <input type="date" className="input" value={takenAt} onChange={(e) => setTakenAt(e.target.value)} />
          </div>
          <div>
            <label className="label">Note</label>
            <input className="input" value={note} onChange={(e) => setNote(e.target.value)} />
          </div>
        </div>
        <button className="btn-primary" disabled={!file || upload.isPending}>
          {upload.isPending ? 'Uploading...' : 'Upload'}
        </button>
      </form>

      {isLoading ? (
        <div>Loading...</div>
      ) : (
        <div className="grid grid-cols-2 md:grid-cols-4 gap-3">
          {data?.items?.map((p) => (
            <div key={p._id} className="card !p-2">
              <a href={assetUrl(p.url)} target="_blank" rel="noreferrer" title="View full size">
                <img
                  src={assetUrl(p.url)}
                  alt={p.note || 'Progress photo'}
                  className="w-full aspect-square object-cover rounded hover:opacity-90 transition-opacity"
                />
              </a>
              <div className="flex items-center justify-between mt-1">
                <div className="text-xs text-slate-500">
                  {new Date(p.takenAt).toLocaleDateString()}
                </div>
                <IconButton
                  icon={Trash2}
                  label="Delete photo"
                  variant="danger"
                  size={16}
                  onClick={() => confirm('Delete photo?') && del.mutate(p._id)}
                />
              </div>
              {p.note && <div className="text-xs">{p.note}</div>}
            </div>
          ))}
          {!data?.items?.length && <div className="text-slate-400">No photos yet.</div>}
        </div>
      )}
    </div>
  );
}
