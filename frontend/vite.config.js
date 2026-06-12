import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    proxy: {
      '/api': 'http://localhost:5109',
      // Uploaded progress photos are served as static files by the backend.
      '/uploads': 'http://localhost:5109',
    },
  },
});
