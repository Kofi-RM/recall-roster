import React, { useRef, useState } from 'react';
import { Alert, Autocomplete, Button, Container, TextField, Stack, Typography } from '@mui/material';
import { useNavigate, Link } from 'react-router-dom';
import { ToolBar } from '../Miscelleneous';
import useRoster from '../hooks/UseRoster';
import api from '../api/api';
import { useUnitAccess } from '../UnitAccess';

const StartRecall = () => {
  const access = useUnitAccess();
  const { rosters: allRosters, loading, error } = useRoster();
  const rosters = allRosters.filter(roster => access.canInitiate(roster.unitId));
  const [selectedRosters, setSelectedRosters] = useState([]);
  const [message, setMessage] = useState('');
  const [duration, setDuration] = useState({ days: 0, hours: 0, minutes: 30 });
  const [pending, setPending] = useState(false);
  const [failure, setFailure] = useState('');
  const [createdId, setCreatedId] = useState(null);
  const submitting = useRef(false);
  const navigate = useNavigate();

  const submit = async (event) => {
    event.preventDefault();
    if (submitting.current || createdId) return;
    const { days, hours, minutes } = duration;
    const values = [days, hours, minutes];
    const totalMinutes = days * 1440 + hours * 60 + minutes;
    if (!selectedRosters.length || !message.trim() || values.some(v => !Number.isInteger(v) || v < 0) ||
        days > 365 || hours > 23 || minutes > 59 || totalMinutes <= 0) {
      setFailure('Choose at least one roster, enter a message, and set a valid duration (up to 365 days).');
      return;
    }
    submitting.current = true;
    setPending(true);
    setFailure('');
    let recallId;
    try {
      const result = await api.post('/Recall', {
        rosterIds: selectedRosters.map(r => r.rosterId), message: message.trim(),
        timeEnded: new Date(Date.now() + totalMinutes * 60000).toISOString()
      });
      recallId = result.data.recallId;
      setCreatedId(recallId);
      const snapshot = await api.get(`/Recall/${recallId}/recipients`);
      let failed = 0;
      // Bounded sequential sends avoid launching an unbounded set of provider requests.
      for (const recipient of snapshot.data.recipients) {
        try {
          await api.post(`/Message/SendMessage/${recipient.contactId}/${recallId}`, { message: message.trim() });
        } catch { failed += 1; }
      }
      if (failed) {
        setFailure(`Recall #${recallId} was created, but ${failed} message submission(s) failed or could not be confirmed. Contact your coordinator; do not create another recall to retry.`);
      } else {
        navigate(`/recallStats/${recallId}`, { state: { submitted: true } });
      }
    } catch (err) {
      setFailure(recallId
        ? `Recall #${recallId} was created, but recipient retrieval or message submission failed. Check its progress before taking further action.`
        : err.response?.data?.message || 'Recall creation could not be confirmed. Check Active Recalls before trying again.');
    } finally {
      submitting.current = false;
      setPending(false);
    }
  };

  return <div>
    <ToolBar />
    <Container component="main" id="main-content" maxWidth="sm" sx={{ py: 5 }}>
      <Typography variant="h4" component="h1" gutterBottom>Initiate recall</Typography>
      <Typography color="text.secondary" sx={{ mb: 3 }}>Recipients and their contact details are saved when this recall begins.</Typography>
      {!access.loading && !access.units.some(unit => unit.canInitiateRecalls) && <Alert severity="info">You need recall initiation permission for a unit.</Alert>}
      {loading && <Alert severity="info">Loading rosters…</Alert>}
      {error && <Alert severity="error">Rosters could not be loaded. Please refresh and try again.</Alert>}
      {failure && <Alert severity="error" sx={{ mb: 2 }}>{failure}</Alert>}
      {createdId && !pending && <Button component={Link} to={`/recallStats/${createdId}`} sx={{ mb: 2 }}>View recall #{createdId}</Button>}
      <Stack component="form" spacing={3} onSubmit={submit}>
        <Autocomplete multiple options={rosters} value={selectedRosters}
          onChange={(_, value) => setSelectedRosters(value)}
          isOptionEqualToValue={(a, b) => a.rosterId === b.rosterId}
          getOptionLabel={r => `${r.name} · ${access.unitName(r.unitId)}`}
          getOptionDisabled={r => selectedRosters.length > 0 && r.unitId !== selectedRosters[0].unitId}
          disabled={pending || !!createdId || loading}
          renderInput={params => <TextField {...params} label="Rosters" helperText="Choose rosters from the same unit. Shared contacts receive one message." />} />
        <TextField label="Message" multiline rows={5} value={message} onChange={e => setMessage(e.target.value)} required inputProps={{ maxLength: 1600 }} disabled={pending || !!createdId} />
        <Typography component="h2" variant="h6">Response deadline</Typography>
        <Stack direction="row" spacing={2}>
          {['days', 'hours', 'minutes'].map(key => <TextField key={key} label={key[0].toUpperCase() + key.slice(1)} type="number"
            value={duration[key]} onChange={e => setDuration({ ...duration, [key]: Number(e.target.value) })}
            inputProps={{ min: 0, max: key === 'days' ? 365 : key === 'hours' ? 23 : 59, step: 1 }}
            disabled={pending || !!createdId} required />)}
        </Stack>
        <Button type="submit" variant="contained" disabled={pending || !!createdId || loading || !!error || !rosters.length}>
          {pending ? 'Creating recall and submitting messages…' : 'Start recall'}
        </Button>
        <Button onClick={() => navigate('/landing')} disabled={pending}>Back to dashboard</Button>
      </Stack>
    </Container>
  </div>;
};
export default StartRecall;
