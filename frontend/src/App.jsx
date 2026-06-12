import { Routes, Route, Navigate } from 'react-router-dom';
import Layout from './components/Layout';
import ProtectedRoute from './components/ProtectedRoute';
import Guest from './pages/Guest';
import Login from './pages/Login';
import Register from './pages/Register';
import Dashboard from './pages/Dashboard';
import Exercises from './pages/Exercises';
import Plans from './pages/Plans';
import PlanEditor from './pages/PlanEditor';
import PlanDetail from './pages/PlanDetail';
import Sessions from './pages/Sessions';
import SessionLogger from './pages/SessionLogger';
import SessionDetail from './pages/SessionDetail';
import Progress from './pages/Progress';
import Profile from './pages/Profile';
import Measurements from './pages/Measurements';
import Photos from './pages/Photos';
import Messages from './pages/Messages';
import Trainer from './pages/Trainer';
import TrainerClientSession from './pages/TrainerClientSession';
import Admin from './pages/Admin';
import System from './pages/System';

export default function App() {
  return (
    <Routes>
      <Route path="/welcome" element={<Guest />} />
      <Route path="/login" element={<Login />} />
      <Route path="/register" element={<Register />} />
      <Route
        element={
          <ProtectedRoute>
            <Layout />
          </ProtectedRoute>
        }
      >
        <Route index element={<Dashboard />} />
        <Route path="exercises" element={<Exercises />} />
        <Route path="plans" element={<Plans />} />
        <Route path="plans/new" element={<PlanEditor />} />
        <Route path="plans/:id" element={<PlanDetail />} />
        <Route path="plans/:id/edit" element={<PlanEditor />} />
        <Route path="sessions" element={<Sessions />} />
        <Route path="sessions/new" element={<SessionLogger />} />
        <Route path="sessions/:id" element={<SessionDetail />} />
        <Route path="progress" element={<Progress />} />
        <Route path="measurements" element={<Measurements />} />
        <Route path="photos" element={<Photos />} />
        <Route path="messages" element={<Messages />} />
        <Route path="profile" element={<Profile />} />
        <Route
          path="trainer/clients/:clientId/sessions/:sessionId"
          element={
            <ProtectedRoute roles={['trainer']}>
              <TrainerClientSession />
            </ProtectedRoute>
          }
        />
        <Route
          path="trainer/*"
          element={
            <ProtectedRoute roles={['trainer', 'admin']}>
              <Trainer />
            </ProtectedRoute>
          }
        />
        <Route
          path="admin/*"
          element={
            <ProtectedRoute roles={['admin']}>
              <Admin />
            </ProtectedRoute>
          }
        />
        <Route
          path="system/*"
          element={
            <ProtectedRoute roles={['admin']}>
              <System />
            </ProtectedRoute>
          }
        />
      </Route>
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
}
