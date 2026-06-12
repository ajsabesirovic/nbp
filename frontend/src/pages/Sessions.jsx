import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useQuery, useMutation, useQueryClient, keepPreviousData } from '@tanstack/react-query';
import toast from 'react-hot-toast';
import { Eye, Trash2 } from 'lucide-react';
import { api } from '../api/client';
import IconButton from '../components/IconButton';

const PAGE_SIZE = 10;

export default function Sessions() {
  const qc = useQueryClient();
  const nav = useNavigate();
  const [page, setPage] = useState(1);

  const { data, isLoading } = useQuery({
    queryKey: ['sessions', page],
    queryFn: () =>
      api.get(`/sessions?page=${page}&limit=${PAGE_SIZE}`).then((r) => r.data),
    placeholderData: keepPreviousData,
  });

  const total = data?.total ?? 0;
  const totalPages = Math.max(1, Math.ceil(total / PAGE_SIZE));

  const del = useMutation({
    mutationFn: (id) => api.delete(`/sessions/${id}`),
    onSuccess: () => {

      if (data?.items?.length === 1 && page > 1) setPage((p) => p - 1);
      qc.invalidateQueries({ queryKey: ['sessions'] });
      toast.success('Deleted');
    },
  });

  return (
    <div className="space-y-4">
      <div className="flex justify-between items-center">
        <h1 className="text-2xl font-bold">Workout sessions</h1>
        <Link to="/sessions/new" className="btn-primary">
          Log new workout
        </Link>
      </div>

      {isLoading ? (
        <div>Loading...</div>
      ) : (
        <>
        <div className="card overflow-x-auto">
          <table className="w-full text-sm">
            <thead className="text-xs uppercase text-slate-500 border-b border-slate-200">
              <tr>
                <th className="text-left py-2">Date</th>
                <th className="text-left py-2">Exercises</th>
                <th className="text-left py-2">Sets</th>
                <th className="text-left py-2">Volume (kg)</th>
                <th className="text-left py-2">Duration</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              {data?.items?.map((s) => (
                <tr
                  key={s._id}
                  className="border-b border-slate-100 hover:bg-slate-50 cursor-pointer"
                  onClick={() => nav(`/sessions/${s._id}`)}
                >
                  <td className="py-2 text-brand-600">
                    {new Date(s.startedAt).toLocaleString()}
                  </td>
                  <td className="py-2">{s.exercises.length}</td>
                  <td className="py-2">{s.completedSets}</td>
                  <td className="py-2">{Math.round(s.totalVolumeKg)}</td>
                  <td className="py-2">{Math.round((s.durationSec || 0) / 60)} min</td>
                  <td className="py-2">
                    <div className="flex items-center gap-1 justify-end">
                      <IconButton
                        icon={Eye}
                        label="View session"
                        variant="brand"
                        onClick={(e) => {
                          e.stopPropagation();
                          nav(`/sessions/${s._id}`);
                        }}
                      />
                      <IconButton
                        icon={Trash2}
                        label="Delete session"
                        variant="danger"
                        onClick={(e) => {
                          e.stopPropagation();
                          if (confirm('Delete session?')) del.mutate(s._id);
                        }}
                      />
                    </div>
                  </td>
                </tr>
              ))}
              {!data?.items?.length && (
                <tr>
                  <td colSpan="6" className="py-6 text-center text-slate-400">
                    No sessions yet.
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>

        {total > 0 && (
          <div className="flex items-center justify-between text-sm">
            <span className="text-slate-500">
              Page {page} of {totalPages} · {total} total
            </span>
            <div className="flex gap-2">
              <button
                className="btn-secondary !py-1.5 !px-3"
                onClick={() => setPage((p) => Math.max(1, p - 1))}
                disabled={page <= 1}
              >
                Previous
              </button>
              <button
                className="btn-secondary !py-1.5 !px-3"
                onClick={() => setPage((p) => Math.min(totalPages, p + 1))}
                disabled={page >= totalPages}
              >
                Next
              </button>
            </div>
          </div>
        )}
        </>
      )}
    </div>
  );
}
