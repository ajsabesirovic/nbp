import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { api } from '../api/client';

const PAGE_SIZE = 25;

export default function System() {
  const [filter, setFilter] = useState('all');
  const [page, setPage] = useState(1);

  const { data, isFetching } = useQuery({
    queryKey: ['admin-logs', filter, page],
    queryFn: () =>
      api
        .get(filter === 'slow' ? '/admin/logs/slow' : '/admin/logs', {
          params: { page, limit: PAGE_SIZE },
        })
        .then((r) => r.data),
    placeholderData: (prev) => prev,
  });

  const items = data?.items || [];
  const total = data?.total || 0;
  const totalPages = Math.max(1, Math.ceil(total / PAGE_SIZE));

  const changeFilter = (f) => {
    setFilter(f);
    setPage(1);
  };

  return (
    <div className="space-y-4">
      <div>
        <h1 className="text-2xl font-bold">System</h1>
        <p className="text-sm text-slate-500">
          Recent HTTP requests captured by the API middleware. Slow requests (&gt; 500&nbsp;ms) are
          highlighted.
        </p>
      </div>

      <div className="flex gap-2">
        <button
          onClick={() => changeFilter('all')}
          className={`px-3 py-1 text-sm rounded ${filter === 'all' ? 'bg-brand-600 text-white' : 'bg-slate-200 border border-slate-300'}`}
        >
          All
        </button>
        <button
          onClick={() => changeFilter('slow')}
          className={`px-3 py-1 text-sm rounded ${filter === 'slow' ? 'bg-brand-600 text-white' : 'bg-slate-200 border border-slate-300'}`}
        >
          Slow only
        </button>
      </div>

      <div className="card overflow-x-auto">
        <table className="w-full text-sm">
          <thead className="text-xs uppercase text-slate-500 border-b">
            <tr>
              <th className="text-left py-2">Time</th>
              <th className="text-left py-2">Method</th>
              <th className="text-left py-2">Path</th>
              <th className="text-left py-2">Status</th>
              <th className="text-left py-2">Duration</th>
            </tr>
          </thead>
          <tbody>
            {items.map((l) => (
              <tr key={l._id} className={`border-b border-slate-100 ${l.slowRequest ? 'bg-amber-500/10' : ''}`}>
                <td className="py-1">{new Date(l.timestamp).toLocaleTimeString()}</td>
                <td className="py-1">{l.method}</td>
                <td className="py-1 font-mono text-xs">{l.path}</td>
                <td className="py-1">{l.statusCode}</td>
                <td className="py-1">{l.durationMs} ms</td>
              </tr>
            ))}
            {!items.length && (
              <tr>
                <td colSpan={5} className="py-3 text-slate-400">
                  No requests logged yet.
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>

      <div className="flex items-center justify-between text-sm">
        <span className="text-slate-500">
          {total.toLocaleString()} request{total === 1 ? '' : 's'} · page {page} of {totalPages}
        </span>
        <div className="flex gap-2 items-center">
          {isFetching && <span className="text-xs text-slate-400">Loading…</span>}
          <button
            className="btn-secondary text-xs"
            disabled={page <= 1}
            onClick={() => setPage((p) => Math.max(1, p - 1))}
          >
            Prev
          </button>
          <button
            className="btn-secondary text-xs"
            disabled={page >= totalPages}
            onClick={() => setPage((p) => p + 1)}
          >
            Next
          </button>
        </div>
      </div>
    </div>
  );
}
