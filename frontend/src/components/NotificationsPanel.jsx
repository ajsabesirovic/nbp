import { useEffect } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Link } from 'react-router-dom';
import { api } from '../api/client';

export default function NotificationsPanel({ open, onClose }) {
  const qc = useQueryClient();

  const { data, isLoading } = useQuery({
    queryKey: ['notifications'],
    queryFn: () => api.get('/notifications').then((r) => r.data),
    enabled: open,
  });

  const markAll = useMutation({
    mutationFn: () => api.post('/notifications/read-all'),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['notifications'] });
      qc.invalidateQueries({ queryKey: ['notif-unread'] });
    },
  });

  const markOne = useMutation({
    mutationFn: (id) => api.post(`/notifications/${id}/read`),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['notifications'] });
      qc.invalidateQueries({ queryKey: ['notif-unread'] });
    },
  });

  useEffect(() => {
    if (!open) return;
    const onKey = (e) => e.key === 'Escape' && onClose();
    window.addEventListener('keydown', onKey);
    return () => window.removeEventListener('keydown', onKey);
  }, [open, onClose]);

  return (
    <>
      {}
      <div
        className={`fixed inset-0 bg-black/40 z-30 transition-opacity ${
          open ? 'opacity-100' : 'opacity-0 pointer-events-none'
        }`}
        onClick={onClose}
      />

      {}
      <aside
        className={`fixed top-0 right-0 h-full w-full max-w-sm bg-slate-100 border-l border-slate-200 shadow-xl z-40 flex flex-col transition-transform duration-200 ${
          open ? 'translate-x-0' : 'translate-x-full'
        }`}
        role="dialog"
        aria-label="Notifications"
      >
        <div className="flex items-center justify-between gap-2 px-4 py-3 border-b border-slate-200">
          <h2 className="text-lg font-bold">Notifications</h2>
          <div className="flex items-center gap-2">
            <button className="btn-secondary text-xs" onClick={() => markAll.mutate()}>
              Mark all read
            </button>
            <button
              className="text-slate-500 hover:text-slate-900 px-2 text-lg leading-none"
              onClick={onClose}
              aria-label="Close"
            >
              ✕
            </button>
          </div>
        </div>

        <div className="flex-1 overflow-auto p-4 space-y-2">
          {isLoading ? (
            <div>Loading...</div>
          ) : (
            <>
              {data?.items?.map((n) => (
                <div
                  key={n._id}
                  className={`card flex items-start justify-between gap-2 ${
                    n.readAt ? 'opacity-60' : ''
                  }`}
                >
                  <div>
                    <div className="font-semibold">{n.title}</div>
                    {n.body && <div className="text-sm text-slate-600">{n.body}</div>}
                    <div className="text-xs text-slate-400 mt-1">
                      {new Date(n.createdAt).toLocaleString()}
                    </div>
                    {n.link && (
                      <Link
                        to={n.link}
                        className="text-xs text-brand-600 hover:underline"
                        onClick={onClose}
                      >
                        Open
                      </Link>
                    )}
                  </div>
                  {!n.readAt && (
                    <button
                      className="btn-secondary text-xs whitespace-nowrap"
                      onClick={() => markOne.mutate(n._id)}
                    >
                      Mark read
                    </button>
                  )}
                </div>
              ))}
              {!data?.items?.length && (
                <div className="text-slate-400">No notifications.</div>
              )}
            </>
          )}
        </div>
      </aside>
    </>
  );
}
