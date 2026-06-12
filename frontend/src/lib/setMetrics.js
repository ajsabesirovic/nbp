
export const SET_METRICS = {
  weightKg: {
    label: 'Weight (kg)',
    cls: 'w-24',
    toView: (v) => v || '',
    fromView: (v) => (v === '' ? undefined : Number(v)),
    display: (v) => (v ?? 0),
  },
  reps: {
    label: 'Reps',
    cls: 'w-20',
    toView: (v) => v || '',
    fromView: (v) => (v === '' ? undefined : Number(v)),
    display: (v) => (v ?? 0),
  },
  durationSec: {
    label: 'Time (min)',
    cls: 'w-24',
    toView: (v) => (v ? +(v / 60).toFixed(2) : ''),
    fromView: (v) => (v === '' ? undefined : Math.round(Number(v) * 60)),
    display: (v) => (v ? +(v / 60).toFixed(2) : '—'),
  },
  distanceM: {
    label: 'Distance (km)',
    cls: 'w-24',
    toView: (v) => (v ? +(v / 1000).toFixed(2) : ''),
    fromView: (v) => (v === '' ? undefined : Math.round(Number(v) * 1000)),
    display: (v) => (v ? +(v / 1000).toFixed(2) : '—'),
  },
};

export function metricsForType(type) {
  switch (type) {
    case 'endurance':
    case 'cardio':
      return ['durationSec', 'distanceM'];
    case 'isometric':
    case 'flexibility':
    case 'balance':
      return ['durationSec'];
    case 'bodyweight':
      return ['reps'];
    default:
      return ['weightKg', 'reps'];
  }
}
