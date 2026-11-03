import { useEffect } from 'react';
import { storeTokens } from '../api/auth';
import type { AuthTokens } from '../types/auth';

/**
 * Handles the /auth/callback route that the API redirects to after Discord OAuth.
 * Tokens arrive in the URL fragment: #access_token=…&refresh_token=…
 */
export default function DiscordCallback() {
  useEffect(() => {
    const hash   = window.location.hash.slice(1);
    const params = new URLSearchParams(hash);

    const accessToken  = params.get('access_token');
    const refreshToken = params.get('refresh_token');

    if (accessToken && refreshToken) {
      // Store tokens then redirect to the app root
      // (Full profile fetch happens in AuthContext on next render)
      const tokens: AuthTokens = {
        accessToken,
        refreshToken,
        expiresIn: 900,
        userId:   '',
        username: '',
        email:    '',
      };
      storeTokens(tokens);
    }

    // Clear the fragment and redirect to root
    window.location.replace('/');
  }, []);

  return <p style={{ color: '#d0d0e8', padding: '32px' }}>Signing you in…</p>;
}
