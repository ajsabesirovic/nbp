/** @type {import('tailwindcss').Config} */
export default {
  content: ['./index.html', './src/**/*.{js,jsx}'],
  theme: {
    extend: {
      colors: {
        // "Red-Line" theme (shadcnthemer) — a dark crimson palette. Values are in
        // oklch with an <alpha-value> placeholder so Tailwind's /opacity modifiers
        // still work. `brand` is the red primary/accent; the `slate` scale is
        // overridden so the whole app uses the theme's dark surfaces + text.
        brand: {
          50: 'oklch(0.205 0.06 16 / <alpha-value>)', // tinted bg for badges / active rows
          100: 'oklch(0.262 0.09 15 / <alpha-value>)',
          500: 'oklch(0.48 0.185 15 / <alpha-value>)', // focus ring
          600: 'oklch(0.44 0.175 14.083 / <alpha-value>)', // primary action (theme primary)
          700: 'oklch(0.53 0.195 16 / <alpha-value>)', // hover / accent text (lighter on dark)
          800: 'oklch(0.64 0.205 18 / <alpha-value>)',
        },
        slate: {
          50: 'oklch(0.094 0.012 19.465 / <alpha-value>)', // page background (theme background)
          100: 'oklch(0.117 0.013 19.149 / <alpha-value>)', // cards (theme card)
          200: 'oklch(0.165 0.02 18 / <alpha-value>)', // elevated surfaces, inputs, subtle borders
          300: 'oklch(0.30 0.06 16 / <alpha-value>)', // stronger borders (the "red line")
          400: 'oklch(0.55 0.025 18 / <alpha-value>)', // dim / tertiary text
          500: 'oklch(0.64 0.02 18 / <alpha-value>)', // muted / secondary text
          600: 'oklch(0.73 0.015 18 / <alpha-value>)',
          700: 'oklch(0.81 0.012 18 / <alpha-value>)', // labels, nav text
          800: 'oklch(0.85 0.008 18 / <alpha-value>)',
          900: 'oklch(0.889 0.001 17.179 / <alpha-value>)', // headings (theme foreground)
        },
      },
    },
  },
  plugins: [],
};
