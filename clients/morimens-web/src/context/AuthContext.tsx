import { createContext, useCallback, useContext, useEffect, useState } from 'react';
import { authApi, clearTokens, getStoredTokens, profileApi, storeTokens } from '../api/auth';
import type { AuthTokens, UserProfile } from '../types/auth';

interface AuthState {
  tokens:   AuthTokens | null;
  profile:  UserProfile | null;
  loading:  boolean;
}

interface AuthContextValue extends AuthState {
  login:    (login: string, password: string) => Promise<void>;
  register: (email: string, username: string, password: string, displayName?: string) => Promise<void>;
  logout:   () => Promise<void>;
  refreshProfile: () => Promise<void>;
}

const AuthContext = createContext<AuthContextValue | null>(null);

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [state, setState] = useState<AuthState>({ tokens: null, profile: null, loading: true });

  // Try to restore session from localStorage on mount
  useEffect(() => {
    const stored = getStoredTokens();
    if (!stored) { setState(s => ({ ...s, loading: false })); return; }

    profileApi.get(stored.accessToken)
      .then(profile => setState({ tokens: stored, profile, loading: false }))
      .catch(async () => {
        // Access token expired — try refresh
        try {
          const fresh = await authApi.refresh(stored.refreshToken);
          storeTokens(fresh);
          const profile = await profileApi.get(fresh.accessToken);
          setState({ tokens: fresh, profile, loading: false });
        } catch {
          clearTokens();
          setState({ tokens: null, profile: null, loading: false });
        }
      });
  }, []);

  const setAuth = useCallback(async (tokens: AuthTokens) => {
    storeTokens(tokens);
    const profile = await profileApi.get(tokens.accessToken);
    setState({ tokens, profile, loading: false });
  }, []);

  const login = useCallback(async (login: string, password: string) => {
    const tokens = await authApi.login(login, password);
    await setAuth(tokens);
  }, [setAuth]);

  const register = useCallback(async (
    email: string, username: string, password: string, displayName?: string
  ) => {
    const tokens = await authApi.register(email, username, password, displayName);
    await setAuth(tokens);
  }, [setAuth]);

  const logout = useCallback(async () => {
    if (state.tokens) {
      try { await authApi.logout(state.tokens.accessToken, state.tokens.refreshToken); } catch {}
    }
    clearTokens();
    setState({ tokens: null, profile: null, loading: false });
  }, [state.tokens]);

  const refreshProfile = useCallback(async () => {
    if (!state.tokens) return;
    const profile = await profileApi.get(state.tokens.accessToken);
    setState(s => ({ ...s, profile }));
  }, [state.tokens]);

  return (
    <AuthContext.Provider value={{ ...state, login, register, logout, refreshProfile }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth(): AuthContextValue {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth must be used inside <AuthProvider>');
  return ctx;
}
