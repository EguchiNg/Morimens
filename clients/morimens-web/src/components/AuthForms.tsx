import { useState } from 'react';
import { authApi } from '../api/auth';
import { useAuth } from '../context/AuthContext';

type FormMode = 'login' | 'register';

export default function AuthForms() {
  const { login, register } = useAuth();
  const [mode,  setMode]  = useState<FormMode>('login');
  const [error, setError] = useState<string | null>(null);
  const [busy,  setBusy]  = useState(false);

  // Login fields
  const [loginVal, setLoginVal] = useState('');
  const [password, setPassword] = useState('');

  // Register extra fields
  const [email,       setEmail]       = useState('');
  const [username,    setUsername]    = useState('');
  const [displayName, setDisplayName] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setBusy(true);
    try {
      if (mode === 'login') {
        await login(loginVal, password);
      } else {
        await register(email, username, password, displayName || undefined);
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Something went wrong.');
    } finally {
      setBusy(false);
    }
  };

  return (
    <div className="auth-wrap">
      <div className="auth-card">
        <h1 className="auth-title">Morimens Calculator</h1>

        <div className="auth-tabs">
          <button className={mode === 'login'    ? 'active' : ''} onClick={() => setMode('login')}>Sign In</button>
          <button className={mode === 'register' ? 'active' : ''} onClick={() => setMode('register')}>Register</button>
        </div>

        <form onSubmit={handleSubmit} className="auth-form">
          {mode === 'register' && (
            <>
              <label>Email
                <input type="email" value={email} onChange={e => setEmail(e.target.value)} required autoComplete="email"/>
              </label>
              <label>Username
                <input type="text" value={username} onChange={e => setUsername(e.target.value)} required minLength={3} maxLength={64}/>
              </label>
              <label>Display Name <span className="optional">(optional)</span>
                <input type="text" value={displayName} onChange={e => setDisplayName(e.target.value)} maxLength={64}/>
              </label>
            </>
          )}

          {mode === 'login' && (
            <label>Email or username
              <input type="text" value={loginVal} onChange={e => setLoginVal(e.target.value)} required autoComplete="username"/>
            </label>
          )}

          <label>Password
            <input type="password" value={password} onChange={e => setPassword(e.target.value)}
                   required minLength={8} autoComplete={mode === 'login' ? 'current-password' : 'new-password'}/>
          </label>

          {error && <p className="auth-error">{error}</p>}

          <button type="submit" className="auth-submit" disabled={busy}>
            {busy ? '…' : mode === 'login' ? 'Sign In' : 'Create Account'}
          </button>
        </form>

        <div className="auth-divider"><span>or</span></div>

        <a href={authApi.discordLoginUrl()} className="discord-btn">
          <svg width="20" height="20" viewBox="0 0 71 55" fill="none" xmlns="http://www.w3.org/2000/svg" aria-hidden="true">
            <path d="M60.1 4.9A58.5 58.5 0 0 0 45.6.4a40.7 40.7 0 0 0-1.8 3.7 54 54 0 0 0-16.3 0A40.7 40.7 0 0 0 25.7.4 58.4 58.4 0 0 0 11.1 5C1.6 19.1-1 32.9.3 46.5a59 59 0 0 0 18 9.1 43 43 0 0 0 3.7-6.1 38.4 38.4 0 0 1-6.1-3c.5-.4 1-.7 1.5-1.1a41.9 41.9 0 0 0 35.9 0c.5.4 1 .8 1.5 1.1a38.4 38.4 0 0 1-6.1 3c1.1 2.1 2.3 4.2 3.7 6.1a58.8 58.8 0 0 0 18-9c1.5-15.7-2.6-29.3-10.3-41.7ZM23.7 38.2c-3.5 0-6.4-3.2-6.4-7.2s2.8-7.2 6.4-7.2 6.5 3.3 6.4 7.2c0 4-2.9 7.2-6.4 7.2Zm23.7 0c-3.5 0-6.4-3.2-6.4-7.2s2.8-7.2 6.4-7.2 6.5 3.3 6.4 7.2c0 4-2.9 7.2-6.4 7.2Z" fill="currentColor"/>
          </svg>
          Continue with Discord
        </a>
      </div>
    </div>
  );
}
