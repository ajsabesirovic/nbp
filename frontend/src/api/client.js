import axios from 'axios';

export const api = axios.create({ baseURL: import.meta.env.VITE_API_URL || '/api/v1' });

export const assetUrl = (u) => {
  if (!u) return u;
  if (/^https?:\/\//.test(u)) return u;
  const apiUrl = import.meta.env.VITE_API_URL;
  if (!apiUrl) return u;
  const origin = apiUrl.replace(/\/api\/v\d+\/?$/, '');
  return origin + (u.startsWith('/') ? u : '/' + u);
};

api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

const aliasIds = (node) => {
  if (Array.isArray(node)) {
    node.forEach(aliasIds);
  } else if (node && typeof node === 'object') {
    if (typeof node.id === 'string' && node._id === undefined) node._id = node.id;
    for (const k in node) aliasIds(node[k]);
  }
  return node;
};

api.interceptors.response.use(
  (res) => {
    aliasIds(res.data);
    return res;
  },
  (err) => {
    if (err.response?.status === 401) {
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      if (!location.pathname.startsWith('/login')) location.href = '/login';
    }
    return Promise.reject(err);
  },
);
