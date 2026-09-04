import React, { useState, useContext, useEffect, useCallback } from 'react';
import { jwtDecode } from 'jwt-decode';

const AuthContext = React.createContext();
const storageKey = 'recallToken';
// Client-side expiry checks improve UX only. The API still validates JWT signatures and authorization.
export const decodeSession = token => {
  try {
    const user = jwtDecode(token);
    return typeof user.exp === 'number' && Number.isFinite(user.exp) && user.exp * 1000 > Date.now()
      ? { token, user } : null;
  } catch { return null; }
};
const readSession = () => {
  const token = localStorage.getItem(storageKey);
  const session = token ? decodeSession(token) : null;
  if (token && !session) localStorage.removeItem(storageKey);
  return session;
};

export const AuthProvider = ({ children }) => {
  const [session, setSession] = useState(readSession);
  const logout = useCallback(() => {
    localStorage.removeItem(storageKey);
    setSession(null);
  }, []);
  const login = useCallback(token => {
    const next = decodeSession(token);
    if (!next) throw new Error('The server returned an invalid or expired session.');
    localStorage.setItem(storageKey, token);
    setSession(next);
  }, []);

  useEffect(() => {
    const sync = event => { if (event.key === storageKey || event.key === null) setSession(readSession()); };
    window.addEventListener('storage', sync);
    return () => window.removeEventListener('storage', sync);
  }, []);
  useEffect(() => {
    if (!session) return;
    let timer;
    const checkExpiry = () => {
      const remaining = session.user.exp * 1000 - Date.now();
      if (remaining <= 0) {
        // A delayed timer must not erase a newer login made in another tab.
        if (localStorage.getItem(storageKey) === session.token) logout();
        else setSession(readSession());
      } else timer = setTimeout(checkExpiry, Math.min(remaining, 2147483647));
    };
    const onFocus = () => { clearTimeout(timer); checkExpiry(); };
    checkExpiry();
    window.addEventListener('focus', onFocus);
    return () => { clearTimeout(timer); window.removeEventListener('focus', onFocus); };
  }, [session, logout]);

  return <AuthContext.Provider value={{ token: session?.token ?? null, user: session?.user ?? null, isLoggedIn: !!session, login, logout }}>
    {children}
  </AuthContext.Provider>;
};
export const useAuth = () => useContext(AuthContext);
