import { Link } from 'react-router-dom';

const VARIANTS = {
  default: 'text-slate-500 hover:text-slate-800 hover:bg-slate-100',
  danger: 'text-red-500 hover:text-red-700 hover:bg-red-50',
  brand: 'text-brand-600 hover:text-brand-800 hover:bg-brand-50',
};

export default function IconButton({
  icon: Icon,
  label,
  variant = 'default',
  to,
  onClick,
  type = 'button',
  disabled = false,
  size = 18,
  className = '',
}) {
  const cls = `inline-flex items-center justify-center p-1.5 rounded-md transition-colors disabled:opacity-40 ${VARIANTS[variant]} ${className}`;
  const content = <Icon size={size} aria-hidden="true" />;

  if (to) {
    return (
      <Link to={to} className={cls} aria-label={label} title={label}>
        {content}
      </Link>
    );
  }
  return (
    <button
      type={type}
      className={cls}
      aria-label={label}
      title={label}
      onClick={onClick}
      disabled={disabled}
    >
      {content}
    </button>
  );
}
