import { Alert, Button, CircularProgress, Paper, TextField } from '@mui/material';
import { useNavigate } from 'react-router-dom';
import axios from 'axios';
import React, { useState, useRef } from 'react';
import { ToolBar, Footer } from './Miscelleneous';
import { useAuth } from './Auth';
import './css/LoginPage.css';

const LoginPage = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [pending, setPending] = useState(false);
  const [error, setError] = useState('');
  const submitting = useRef(false);
  const { login } = useAuth();
  const navigate = useNavigate();

  const handleLogin = async (event) => {
    event.preventDefault();
    if (submitting.current) return;
    submitting.current = true;
    setPending(true);
    setError('');
    try {
      const response = await axios.post('http://localhost:5000/api/auth/login', { email: email.trim(), password });
      login(response.data.token);
      navigate('/landing', { replace: true });
    } catch (failure) {
      setError(failure.response?.status === 401 || failure.response?.status === 400
        ? 'The email or password is incorrect. Please try again.'
        : 'Unable to sign in right now. Please try again shortly.');
    } finally {
      submitting.current = false;
      setPending(false);
    }
  };

  return (
    <div className="rr-page rr-login-page">
      <ToolBar />
      <main id="main-content" className="rr-login-main">
        <div className="rr-login-intro"><p className="rr-eyebrow">YOUR TEAM, WITHIN REACH</p><h1>Welcome back.</h1><p>Sign in to manage rosters, coordinate recalls, and follow your team’s responses.</p></div>
        <Paper component="section" elevation={0} className="rr-login-card">
          <h2>Officer login</h2>
          <p>Enter your account details to continue.</p>
          <form onSubmit={handleLogin}>
            {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
            <TextField label="Email" type="email" autoComplete="username" fullWidth margin="normal" value={email} onChange={(event) => setEmail(event.target.value)} required disabled={pending} />
            <TextField label="Password" type="password" autoComplete="current-password" fullWidth margin="normal" value={password} onChange={(event) => setPassword(event.target.value)} required disabled={pending} />
            <Button type="submit" variant="contained" fullWidth disabled={pending} sx={{ mt: 3, py: 1.5, bgcolor: '#1c2347' }}>
              {pending ? <><CircularProgress size={18} color="inherit" sx={{ mr: 1 }} />Signing in…</> : 'Sign in'}
            </Button>
            <p className="rr-login-help">Need access or help with your password? Contact your administrator.</p>
          </form>
        </Paper>
      </main>
      <Footer />
    </div>
  );
};
export default LoginPage;
