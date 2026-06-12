import { Link, useParams, useNavigate } from 'react-router-dom';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import toast from 'react-hot-toast';
import { Trash2 } from 'lucide-react';
import { api } from '../api/client';
import SessionView from '../components/SessionView';

export default function SessionDetail() {
  const { id } = useParams();
  const nav = useNavigate();
  const qc = useQueryClient();

  const { data: s, isLoading, isError } = useQuery({
    queryKey: ['session', id],
    queryFn: () => api.get(`/sessions/${id}`).then((r) => r.data),
  });

  const del = useMutation({
    mutationFn: () => api.delete(`/sessions/${id}`),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['sessions'] });
      toast.success('Deleted');
      nav('/sessions');
    },
    onError: (err) => toast.error(err.response?.data?.error || 'Failed to delete'),
  });

  if (isLoading) return <div>Loading...</div>;
  if (isError || !s) {
    return (
      <div className="space-y-3">
        <p className="text-slate-500">Session not found.</p>
        <Link to="/sessions" className="btn-secondary text-sm">
          ← Back to sessions
        </Link>
      </div>
    );
  }

  return (
    <div className="space-y-4">
      <div className="flex justify-between items-center">
        <div>
          <Link to="/sessions" className="text-sm text-brand-600">
            ← Back to sessions
          </Link>
          <h1 className="text-2xl font-bold mt-1">
            {new Date(s.startedAt).toLocaleString()}
          </h1>
        </div>
        <button
          className="btn-danger text-sm inline-flex items-center gap-1.5"
          onClick={() => confirm('Delete this session?') && del.mutate()}
          disabled={del.isPending}
        >
          <Trash2 size={16} aria-hidden="true" />
          Delete
        </button>
      </div>

      <SessionView session={s} />
    </div>
  );
}
