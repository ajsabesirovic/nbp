import { useEffect, useState } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import toast from 'react-hot-toast';
import { api } from '../api/client';
import { useAuth } from '../context/AuthContext';

export default function Messages() {
  const { user } = useAuth();
  const qc = useQueryClient();
  const [otherId, setOtherId] = useState('');
  const [body, setBody] = useState('');
  const [newRecipient, setNewRecipient] = useState('');

  const threads = useQuery({
    queryKey: ['msg-threads'],
    queryFn: () => api.get('/messages/threads').then((r) => r.data),
  });

  const thread = useQuery({
    queryKey: ['msg-thread', otherId],
    queryFn: () => api.get(`/messages/thread/${otherId}`).then((r) => r.data),
    enabled: !!otherId,
  });

  const contacts = useQuery({
    queryKey: ['msg-contacts'],
    queryFn: () => api.get('/messages/contacts').then((r) => r.data).catch(() => ({ items: [] })),
  });

  useEffect(() => {
    if (!otherId && threads.data?.items?.length) {
      setOtherId(threads.data.items[0].otherUserId);
    }
  }, [threads.data, otherId]);

  const send = useMutation({
    mutationFn: (payload) => api.post('/messages', payload),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['msg-thread', otherId] });
      qc.invalidateQueries({ queryKey: ['msg-threads'] });
      qc.invalidateQueries({ queryKey: ['msg-unread'] });
      setBody('');
    },
    onError: (err) => toast.error(err.response?.data?.error || 'Send failed'),
  });

  const submit = (e) => {
    e.preventDefault();
    if (!otherId || !body.trim()) return;
    send.mutate({ toUserId: otherId, body });
  };

  const otherName =
    threads.data?.items?.find((t) => t.otherUserId === otherId)?.otherUserName ||
    contacts.data?.items?.find((u) => u.id === otherId)?.name ||
    thread.data?.items?.find((m) => m.fromUserId === otherId)?.fromName ||
    'Conversation';

  const startNew = () => {
    if (newRecipient) {
      setOtherId(newRecipient);
      setNewRecipient('');
    }
  };

  return (
    <div className="space-y-4">
      <h1 className="text-2xl font-bold">Messages</h1>
      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
        <div className="card !p-3 space-y-2 md:col-span-1">
          <h2 className="font-semibold text-sm">Conversations</h2>
          <ul className="space-y-1">
            {threads.data?.items?.map((t) => (
              <li key={t.otherUserId}>
                <button
                  className={`w-full text-left px-2 py-2 rounded-md text-sm ${
                    otherId === t.otherUserId ? 'bg-brand-50 text-brand-700' : 'hover:bg-slate-50'
                  }`}
                  onClick={() => setOtherId(t.otherUserId)}
                >
                  <div className="font-medium flex justify-between">
                    <span>{t.otherUserName}</span>
                    {t.unreadCount > 0 && (
                      <span className="text-xs bg-brand-600 text-white rounded-full px-1.5">
                        {t.unreadCount}
                      </span>
                    )}
                  </div>
                  <div className="text-xs text-slate-500 truncate">{t.lastBody}</div>
                </button>
              </li>
            ))}
            {!threads.data?.items?.length && <div className="text-xs text-slate-400">No threads.</div>}
          </ul>
          <div className="border-t border-slate-100 pt-2">
            <label className="label text-xs">Start new conversation</label>
            <select
              className="input !text-xs !py-1"
              value={newRecipient}
              onChange={(e) => setNewRecipient(e.target.value)}
            >
              <option value="">—</option>
              {contacts.data?.items?.map((u) => (
                <option key={u.id} value={u.id}>
                  {u.name}
                </option>
              ))}
            </select>
            {!contacts.data?.items?.length && (
              <div className="text-xs text-slate-400 mt-1">No one to message yet.</div>
            )}
            <button className="btn-secondary text-xs mt-1 w-full" onClick={startNew} disabled={!newRecipient}>
              Open
            </button>
          </div>
        </div>

        <div className="card md:col-span-2 flex flex-col h-[70vh]">
          {!otherId ? (
            <div className="text-slate-400">Pick or start a conversation.</div>
          ) : (
            <>
              <div className="border-b border-slate-100 pb-2 mb-2">
                <div className="font-semibold">{otherName}</div>
              </div>
              <div className="flex-1 overflow-y-auto space-y-2 pr-2">
                {thread.data?.items?.map((m) => {
                  const mine = m.fromUserId === (user?.id ?? user?._id);
                  return (
                    <div key={m._id} className={`flex ${mine ? 'justify-end' : 'justify-start'}`}>
                      <div
                        className={`max-w-[70%] px-3 py-2 rounded-lg text-sm ${
                          mine ? 'bg-brand-600 text-white' : 'bg-slate-200'
                        }`}
                      >
                        <div className="text-xs opacity-70 mb-0.5">
                          {mine ? 'You' : m.fromName} · {new Date(m.createdAt).toLocaleString()}
                        </div>
                        <div>{m.body}</div>
                      </div>
                    </div>
                  );
                })}
                {!thread.data?.items?.length && (
                  <div className="text-slate-400 text-sm">No messages yet — say hi.</div>
                )}
              </div>
              <form onSubmit={submit} className="mt-3 flex gap-2">
                <input
                  className="input flex-1"
                  placeholder="Type a message..."
                  value={body}
                  onChange={(e) => setBody(e.target.value)}
                />
                <button className="btn-primary" disabled={send.isPending || !body.trim()}>
                  Send
                </button>
              </form>
            </>
          )}
        </div>
      </div>
    </div>
  );
}
