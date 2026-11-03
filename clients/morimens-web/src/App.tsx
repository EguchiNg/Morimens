import { AuthProvider, useAuth } from './context/AuthContext';
import AuthForms           from './components/AuthForms';
import DamageCalculator    from './components/DamageCalculator';
import DiscordCallback     from './components/DiscordCallback';
import './App.css';

function AppShell() {
  const { tokens, profile, loading, logout } = useAuth();

  // Handle Discord OAuth redirect
  if (window.location.pathname === '/auth/callback') {
    return <DiscordCallback />;
  }

  if (loading) {
    return (
      <div className="loading-screen">
        <div className="loading-spinner" aria-label="Loading…" />
      </div>
    );
  }

  if (!tokens) {
    return <AuthForms />;
  }

  return (
    <>
      <nav className="app-nav">
        <span className="nav-brand">Morimens</span>
        <div className="nav-right">
          <span className="nav-user">{profile?.displayName ?? profile?.username ?? '…'}</span>
          <button className="nav-logout" onClick={logout}>Sign Out</button>
        </div>
      </nav>
      <DamageCalculator />
    </>
  );
}

export default function App() {
  return (
    <AuthProvider>
      <AppShell />
    </AuthProvider>
  );
}
