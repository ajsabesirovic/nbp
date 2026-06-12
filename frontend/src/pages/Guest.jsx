import { Link } from 'react-router-dom';

const FEATURES = [
  {
    icon: '📋',
    title: 'Training Plans',
    desc: 'Follow structured workout plans built by trainers, or craft your own routine week by week.',
  },
  {
    icon: '🏋️',
    title: 'Log Every Session',
    desc: 'Track sets, reps and weight in seconds. Your whole training history, always at hand.',
  },
  {
    icon: '📈',
    title: 'Visual Progress',
    desc: 'Charts for volume, personal bests and body measurements show exactly how far you have come.',
  },
  {
    icon: '📸',
    title: 'Progress Photos',
    desc: 'Keep a private timeline of progress photos and compare transformations over time.',
  },
  {
    icon: '💬',
    title: 'Talk to Your Trainer',
    desc: 'Message your trainer directly, get feedback, and stay accountable between sessions.',
  },
  {
    icon: '🔔',
    title: 'Smart Reminders',
    desc: 'Stay on track with notifications for upcoming workouts and trainer updates.',
  },
];

const STEPS = [
  { n: '1', title: 'Create an account', desc: 'Sign up free in under a minute — no card required.' },
  { n: '2', title: 'Pick or build a plan', desc: 'Choose a ready-made plan or design your own workouts.' },
  { n: '3', title: 'Train & track', desc: 'Log sessions and watch your progress charts grow.' },
];

function GuestNav() {
  return (
    <header className="sticky top-0 z-20 bg-slate-100/80 backdrop-blur border-b border-slate-200">
      <div className="max-w-6xl mx-auto px-4 py-3 flex items-center justify-between">
        <div className="text-xl font-bold text-brand-600">FitJourney</div>
        <div className="flex items-center gap-2">
          <Link to="/login" className="btn-secondary !py-1.5 !px-4 text-sm">
            Sign in
          </Link>
          <Link to="/register" className="btn-primary !py-1.5 !px-4 text-sm">
            Get started
          </Link>
        </div>
      </div>
    </header>
  );
}

export default function Guest() {
  return (
    <div className="min-h-screen bg-slate-50 flex flex-col">
      <GuestNav />

      {}
      <section className="relative overflow-hidden">
        <div className="absolute inset-0 bg-gradient-to-br from-brand-50 via-slate-100 to-slate-50" />
        <div className="absolute -top-24 -right-24 h-72 w-72 rounded-full bg-brand-600 blur-3xl opacity-20" />
        <div className="relative max-w-6xl mx-auto px-4 py-20 md:py-28 text-center">
          <span className="badge bg-brand-100 text-brand-700 mb-5">Your personal training companion</span>
          <h1 className="text-4xl md:text-6xl font-extrabold tracking-tight text-slate-900">
            Train smarter.
            <span className="text-brand-600"> Track everything.</span>
          </h1>
          <p className="mt-5 max-w-2xl mx-auto text-lg text-slate-600">
            FitJourney brings your workout plans, session logs, progress charts and your trainer together
            in one clean, focused place.
          </p>
          <div className="mt-8 flex flex-wrap items-center justify-center gap-3">
            <Link to="/register" className="btn-primary !px-6 !py-3 text-base">
              Start free
            </Link>
            <Link to="/login" className="btn-secondary !px-6 !py-3 text-base">
              I already have an account
            </Link>
          </div>
          <p className="mt-4 text-xs text-slate-400">No credit card · Free to get started</p>
        </div>
      </section>

      {}
      <section className="max-w-6xl mx-auto w-full px-4 py-16">
        <div className="text-center mb-12">
          <h2 className="text-3xl font-bold text-slate-900">Everything you need to keep going</h2>
          <p className="mt-3 text-slate-600">One app for the whole journey — from first rep to new personal best.</p>
        </div>
        <div className="grid gap-5 sm:grid-cols-2 lg:grid-cols-3">
          {FEATURES.map((f) => (
            <div
              key={f.title}
              className="card hover:shadow-md hover:-translate-y-0.5 transition will-change-transform"
            >
              <div className="text-3xl mb-3">{f.icon}</div>
              <h3 className="font-semibold text-slate-900 mb-1">{f.title}</h3>
              <p className="text-sm text-slate-600 leading-relaxed">{f.desc}</p>
            </div>
          ))}
        </div>
      </section>

      {}
      <section className="bg-slate-100 border-y border-slate-200">
        <div className="max-w-6xl mx-auto w-full px-4 py-16">
          <div className="text-center mb-12">
            <h2 className="text-3xl font-bold text-slate-900">Get going in three steps</h2>
          </div>
          <div className="grid gap-8 md:grid-cols-3">
            {STEPS.map((s) => (
              <div key={s.n} className="text-center">
                <div className="mx-auto mb-4 flex h-12 w-12 items-center justify-center rounded-full bg-brand-600 text-white text-lg font-bold">
                  {s.n}
                </div>
                <h3 className="font-semibold text-slate-900 mb-1">{s.title}</h3>
                <p className="text-sm text-slate-600">{s.desc}</p>
              </div>
            ))}
          </div>
        </div>
      </section>

      {}
      <section className="max-w-6xl mx-auto w-full px-4 py-16">
        <div className="relative overflow-hidden rounded-2xl bg-brand-600 px-6 py-12 md:py-16 text-center text-white">
          <div className="absolute -bottom-16 -left-16 h-64 w-64 rounded-full bg-brand-500 opacity-50 blur-2xl" />
          <div className="relative">
            <h2 className="text-3xl md:text-4xl font-bold">Ready to start your journey?</h2>
            <p className="mt-3 text-brand-50 max-w-xl mx-auto">
              Join FitJourney and turn every workout into measurable progress.
            </p>
            <div className="mt-7 flex flex-wrap items-center justify-center gap-3">
              <Link
                to="/register"
                className="btn !bg-white !text-brand-700 hover:!bg-brand-50 !px-6 !py-3 text-base"
              >
                Create free account
              </Link>
              <Link
                to="/login"
                className="btn !border !border-white/70 !text-white hover:!bg-white/10 !px-6 !py-3 text-base"
              >
                Sign in
              </Link>
            </div>
          </div>
        </div>
      </section>

      <footer className="mt-auto border-t border-slate-200 bg-slate-100">
        <div className="max-w-6xl mx-auto px-4 py-6 flex flex-col sm:flex-row items-center justify-between gap-2 text-sm text-slate-500">
          <span className="font-semibold text-brand-600">FitJourney</span>
          <span>© {new Date().getFullYear()} FitJourney. Train smarter.</span>
        </div>
      </footer>
    </div>
  );
}
