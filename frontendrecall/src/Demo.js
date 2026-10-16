import React, { useEffect, useRef, useState } from 'react';
import { Alert, Button, Checkbox, FormControlLabel, TextField } from '@mui/material';
import './css/Demo.css';

const base = (process.env.REACT_APP_API_URL || '/api').replace(/\/$/, '');
const messages = {
  recall: 'Recall Roster DEMO: A test recall has started. Please check in with your coordinator. No action is required. Reply STOP to opt out.',
  reminder: 'Recall Roster DEMO: This is a test recall reminder. No action is required. Thanks for trying the demo! Reply STOP to opt out.',
};

export default function Demo() {
  const [token, setToken] = useState(() => sessionStorage.getItem('recallDemoSession') || '');
  const [phone, setPhone] = useState('');
  const [consent, setConsent] = useState(false);
  const [code, setCode] = useState('');
  const [verified, setVerified] = useState(false);
  const [remaining, setRemaining] = useState(2);
  const [template, setTemplate] = useState('recall');
  const [notice, setNotice] = useState(null);
  const [busy, setBusy] = useState(false);
  const [loading, setLoading] = useState(Boolean(token));
  const [history, setHistory] = useState([]);
  const inFlight = useRef(false);

  async function request(path, body, session = token) {
    const response = await fetch(`${base}/demo/${path}`, {
      method: body ? 'POST' : 'GET',
      headers: { 'Content-Type': 'application/json', 'X-Demo-Session': session },
      ...(body ? { body: JSON.stringify(body) } : {}),
    });
    const data = await response.json().catch(() => ({}));
    if (!response.ok) {
      const error = new Error(data.message || 'The demo is unavailable. Please try again later.');
      error.status = response.status;
      throw error;
    }
    return data;
  }

  useEffect(() => {
    if (!token) return;
    let active = true;
    request('session', null, token).then(data => {
      if (active) { setVerified(data.verified); setRemaining(data.remaining); setPhone(data.phone); }
    }).catch(error => {
      if (active) {
        setNotice({ severity: 'error', text: error.message });
        if (error.status === 401) { setToken(''); sessionStorage.removeItem('recallDemoSession'); setPhone(''); }
      }
    }).finally(() => { if (active) setLoading(false); });
    return () => { active = false; };
    // Only restore when the page first opens; verification manages state afterward.
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  async function perform(action) {
    if (inFlight.current) return;
    inFlight.current = true;
    setBusy(true); setNotice(null);
    try { await action(); }
    catch (error) {
      setNotice({ severity: 'error', text: error.message || 'Unable to connect. Please try again.' });
      if (error.status === 401) { setToken(''); setVerified(false); setPhone(''); sessionStorage.removeItem('recallDemoSession'); }
    }
    finally { inFlight.current = false; setBusy(false); }
  }

  function start(event) {
    event.preventDefault();
    perform(async () => {
      const result = await request('verify/start', { phone, consent });
      sessionStorage.setItem('recallDemoSession', result.token);
      setToken(result.token);
      setNotice({ severity: 'success', text: result.message });
    });
  }
  function verify(event) {
    event.preventDefault();
    perform(async () => {
      const result = await request('verify/check', { code });
      setVerified(true); setRemaining(result.remaining); setCode('');
      setNotice({ severity: 'success', text: 'Phone verified. You can now send a test recall.' });
    });
  }
  function send() {
    perform(async () => {
      try {
        const result = await request('send', { template });
        setRemaining(result.remaining);
        setHistory(items => [{ label: template === 'recall' ? 'Test recall' : 'Test reminder', time: new Date().toLocaleTimeString() }, ...items]);
        setNotice({ severity: 'success', text: result.message });
      } catch (error) {
        // A provider/network failure may have consumed a send. Refresh from the server,
        // never assume failure refunds the quota or automatically retry an SMS.
        try { const result = await request('session'); setRemaining(result.remaining); } catch { setRemaining(0); }
        throw error;
      }
    });
  }

  return <div className="rr-demo">
    <header className="demo-nav"><strong>Recall Roster</strong><span>LIVE DEMO</span></header>
    <main className="demo-main">
      <header><p className="demo-eyebrow">TRY A RECALL</p><h1>Send a test text to your phone.</h1><p>Verify your number, then try two real SMS sends. No account needed.</p></header>
      {notice && <Alert severity={notice.severity} role="alert">{notice.text}</Alert>}
      {loading ? <p role="status">Restoring your demo…</p> : <div className="demo-grid">
        <section className="demo-panel" aria-labelledby="demo-phone-heading">
          <h2 id="demo-phone-heading">1. Your demo contact</h2>
          {verified ? <><p className="demo-verified">Phone verified</p><p>{phone}</p><p>Only your verified number receives texts. No staff contacts are used.</p></> : token ?
            <form onSubmit={verify}>
              <TextField label="Verification code" value={code} onChange={e => setCode(e.target.value)} required fullWidth autoComplete="one-time-code" inputProps={{ inputMode: 'numeric', pattern: '[0-9]{4,10}', maxLength: 10 }} />
              <Button type="submit" variant="contained" disabled={busy}>{busy ? 'Checking…' : 'Verify phone'}</Button>
              <Button disabled={busy} onClick={() => { setToken(''); setCode(''); setPhone(''); sessionStorage.removeItem('recallDemoSession'); }}>Use another number or request a new code</Button>
            </form> : <form onSubmit={start}>
              <TextField label="Phone number" type="tel" placeholder="+12025550123" helperText="Include + and your country code. Default demo region: +1." value={phone} onChange={e => setPhone(e.target.value)} fullWidth required inputProps={{ maxLength: 16, pattern: '\\+[1-9][0-9]{7,14}' }} />
              <FormControlLabel control={<Checkbox checked={consent} onChange={e => setConsent(e.target.checked)} required />} label="This is my number. I agree to receive a verification code and up to two demo texts. Message and data rates may apply." />
              <Button type="submit" variant="contained" disabled={busy || !consent}>{busy ? 'Sending code…' : 'Send verification code'}</Button>
            </form>}
        </section>
        <section className="demo-panel" aria-labelledby="demo-recall-heading">
          <div className="demo-panel-title"><h2 id="demo-recall-heading">2. Start a test recall</h2><span className="demo-quota">{remaining} / 2 sends left</span></div>
          <label htmlFor="demo-message">Message</label>
          <select id="demo-message" value={template} onChange={e => setTemplate(e.target.value)} disabled={busy}>
            <option value="recall">Initial recall</option><option value="reminder">Recall reminder</option>
          </select>
          <blockquote>{messages[template]}</blockquote>
          <Button variant="contained" disabled={!verified || busy || remaining === 0} onClick={send}>{busy && verified ? 'Sending…' : 'Send test SMS'}</Button>
          <p className="demo-note">{remaining === 0 ? 'Your two demo attempts are used. Thanks for trying Recall Roster!' : 'Two send attempts per phone number. Refreshing does not reset the limit. Failed attempts may still count.'}</p>
        </section>
      </div>}
      {history.length > 0 && <section className="demo-panel demo-history"><h2>This session</h2><ul>{history.map((item, index) => <li key={index}><strong>{item.label}</strong><span>Submitted to SMS provider · {item.time}</span></li>)}</ul></section>}
      <footer>This demo shows phone verification and outgoing recall texts. Staff management and reply tracking are available in the full app.</footer>
    </main>
  </div>;
}
