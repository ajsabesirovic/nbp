import { Link, useParams } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { api } from '../api/client';
import SessionView from '../components/SessionView';

export default function TrainerClientSession() {
  const { clientId, sessionId } = useParams();

  const { data: s, isLoading, isError } = useQuery({
    queryKey: ['client-session', clientId, sessionId],
    queryFn: () =>
      api.get(`/trainer/clients/${clientId}/sessions/${sessionId}`).then((r) => r.data),
  });

  if (isLoading) return <div>Loading...</div>;
  if (isError || !s) {
    return (
      <div className="space-y-3">
        <p className="text-slate-500">Session not found.</p>
        <Link to="/trainer" className="btn-secondary text-sm">
          ← Back to clients
        </Link>
      </div>
    );
  }

  return (
    <div className="space-y-4">
      <div>
        <Link to="/trainer" className="text-sm text-brand-600">
          ← Back to clients
        </Link>
        <h1 className="text-2xl font-bold mt-1">{new Date(s.startedAt).toLocaleString()}</h1>
      </div>

      <SessionView session={s} />
    </div>
  );
}
