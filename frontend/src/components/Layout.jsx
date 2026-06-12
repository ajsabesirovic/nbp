import { useState } from 'react';
import { NavLink, Outlet } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { useAuth } from '../context/AuthContext';
import { api } from '../api/client';
import NotificationsPanel from './NotificationsPanel';

function NavItem({ to, children }) {
  return (
    <NavLink
      to={to}
      end
      className={({ isActive }) =>
        `px-3 py-2 rounded-md text-sm font-medium ${
          isActive ? 'bg-brand-600 text-white' : 'text-slate-700 hover:bg-slate-200'
        }`
      }
    >
      {children}
    </NavLink>
  );
}

function Badge({ count }) {
  if (!count) return null;
  return (
    <span className="ml-1 inline-flex items-center justify-center text-[10px] bg-red-500 text-white rounded-full min-w-[18px] h-[18px] px-1">
      {count > 99 ? '99+' : count}
    </span>
  );
}

export default function Layout() {
  const { user, logout } = useAuth();
  const [notifOpen, setNotifOpen] = useState(false);

  const notifUnread = useQuery({
    queryKey: ['notif-unread'],
    queryFn: () => api.get('/notifications/unread-count').then((r) => r.data.count),
    refetchInterval: 30000,
  });

  const msgUnread = useQuery({
    queryKey: ['msg-unread'],
    queryFn: () => api.get('/messages/unread-count').then((r) => r.data.count),
    refetchInterval: 30000,
  });

  const role = user?.role;
  const isAdmin = role === 'admin';
  const isTrainer = role === 'trainer';
  const isUser = role === 'user';

  return (
    <div className="min-h-screen flex flex-col">
      <header className="bg-slate-100 border-b border-slate-200 sticky top-0 z-10">
        <div className="max-w-7xl mx-auto px-4 py-3 flex items-center gap-4">
          <div className="text-xl font-bold text-brand-600">FitJourney</div>
          <nav className="flex gap-1 flex-1 flex-wrap">
            {!isTrainer && <NavItem to="/">Dashboard</NavItem>}
            <NavItem to="/exercises">Exercises</NavItem>
            <NavItem to="/plans">Plans</NavItem>
            {isUser && (
              <>
                <NavItem to="/sessions">Sessions</NavItem>
                <NavItem to="/progress">Progress</NavItem>
                <NavItem to="/measurements">Measurements</NavItem>
                <NavItem to="/photos">Photos</NavItem>
              </>
            )}
            <NavItem to="/messages">
              Messages
              <Badge count={msgUnread.data} />
            </NavItem>
            {isTrainer && <NavItem to="/trainer">Clients</NavItem>}
            {isAdmin && <NavItem to="/trainer">Trainer</NavItem>}
            {isAdmin && <NavItem to="/admin">Users</NavItem>}
            {isAdmin && <NavItem to="/system">System</NavItem>}
          </nav>
          <div className="flex items-center gap-3">
            <button
              type="button"
              onClick={() => setNotifOpen(true)}
              className="text-slate-700 hover:bg-slate-200 px-2 py-2 rounded-md text-sm"
              title="Notifications"
            >
              🔔
              <Badge count={notifUnread.data} />
            </button>
            {isAdmin ? (

              <span className="text-sm text-slate-600">
                {user?.name} <span className="text-xs text-slate-400">({user?.role})</span>
              </span>
            ) : (
              <NavLink to="/profile" className="text-sm text-slate-600 hover:text-slate-900">
                {user?.name} <span className="text-xs text-slate-400">({user?.role})</span>
              </NavLink>
            )}
            <button onClick={logout} className="btn-secondary !py-1 !px-3 text-xs">
              Logout
            </button>
          </div>
        </div>
      </header>
      <main className="flex-1 max-w-7xl mx-auto w-full px-4 py-6">
        <Outlet />
      </main>
      <NotificationsPanel open={notifOpen} onClose={() => setNotifOpen(false)} />
    </div>
  );
}
