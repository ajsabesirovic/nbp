# FitJourney — Frontend

React 18 + Vite + Tailwind CSS + React Query + Recharts.

## Setup

```bash
npm install
npm run dev   # http://localhost:5173
```

Vite proxies `/api` to `http://localhost:4000` — start the backend first.

## Routes

- `/login`, `/register`
- `/` dashboard
- `/exercises` — library with search and admin CRUD
- `/plans` — public / mine / assigned tabs; plan editor at `/plans/new` and `/plans/:id/edit`
- `/sessions` — log new (`/sessions/new`) and history
- `/progress` — weekly volume, exercise progression, muscle balance, body measurements
- `/profile`
- `/trainer` — client list, plan completion, recent sessions
- `/admin` — users, public plans moderation, request logs
