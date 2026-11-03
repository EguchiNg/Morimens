import type { AuthTokens, SavedSimulation, UserProfile, UserSettings } from '../types/auth';

const BASE_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:5000';

// ── Token storage ─────────────────────────────────────────────────────────────

const STORAGE_KEY = 'morimens_auth';

export function getStoredTokens(): AuthTokens | null {
  try {
    const raw = localStorage.getItem(STORAGE_KEY);
    return raw ? (JSON.parse(raw) as AuthTokens) : null;
  } catch { return null; }
}

export function storeTokens(tokens: AuthTokens): void {
  try { localStorage.setItem(STORAGE_KEY, JSON.stringify(tokens)); } catch {}
}

export function clearTokens(): void {
  try { localStorage.removeItem(STORAGE_KEY); } catch {}
}

// ── HTTP helpers ──────────────────────────────────────────────────────────────

async function request<T>(
  path: string,
  opts: RequestInit = {},
  accessToken?: string,
): Promise<T> {
  const headers: Record<string, string> = {
    'Content-Type': 'application/json',
    ...(accessToken ? { Authorization: `Bearer ${accessToken}` } : {}),
  };
  const res = await fetch(`${BASE_URL}${path}`, { ...opts, headers });
  if (!res.ok) {
    const text = await res.text();
    throw new Error(`${opts.method ?? 'GET'} ${path} → ${res.status}: ${text}`);
  }
  if (res.status === 204) return undefined as unknown as T;
  return res.json() as Promise<T>;
}

// ── Auth endpoints ────────────────────────────────────────────────────────────

export const authApi = {
  register: (email: string, username: string, password: string, displayName?: string) =>
    request<AuthTokens>('/api/auth/register', {
      method: 'POST',
      body: JSON.stringify({ email, username, password, displayName }),
    }),

  login: (login: string, password: string) =>
    request<AuthTokens>('/api/auth/login', {
      method: 'POST',
      body: JSON.stringify({ login, password }),
    }),

  refresh: (refreshToken: string) =>
    request<AuthTokens>('/api/auth/refresh', {
      method: 'POST',
      body: JSON.stringify({ refreshToken }),
    }),

  logout: (accessToken: string, refreshToken: string) =>
    request<void>('/api/auth/logout', {
      method: 'POST',
      body: JSON.stringify({ refreshToken }),
    }, accessToken),

  discordLoginUrl: () => `${BASE_URL}/api/auth/discord`,
};

// ── Profile endpoints ─────────────────────────────────────────────────────────

export const profileApi = {
  get: (token: string) =>
    request<UserProfile>('/api/profile', {}, token),

  update: (token: string, body: { displayName?: string; avatarUrl?: string }) =>
    request<UserProfile>('/api/profile', { method: 'PATCH', body: JSON.stringify(body) }, token),

  getSettings: (token: string) =>
    request<UserSettings>('/api/profile/settings', {}, token),

  updateSettings: (token: string, settings: Partial<UserSettings>) =>
    request<UserSettings>('/api/profile/settings', {
      method: 'PUT',
      body: JSON.stringify(settings),
    }, token),
};

// ── Simulation endpoints ──────────────────────────────────────────────────────

export const simulationApi = {
  list: (token: string) =>
    request<SavedSimulation[]>('/api/simulation', {}, token),

  get: (token: string, id: number) =>
    request<SavedSimulation>(`/api/simulation/${id}`, {}, token),

  save: (token: string, name: string, inputs: unknown, result: unknown) =>
    request<SavedSimulation>('/api/simulation', {
      method: 'POST',
      body: JSON.stringify({ name, inputsJson: inputs, resultJson: result }),
    }, token),

  delete: (token: string, id: number) =>
    request<void>(`/api/simulation/${id}`, { method: 'DELETE' }, token),
};
