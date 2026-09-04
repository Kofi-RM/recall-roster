import React, { useEffect, useState } from 'react';
import { useLocation, useNavigate, useParams } from 'react-router-dom';
import { Alert, Box, Button, Container, LinearProgress, Paper, Tab, Tabs, Typography } from '@mui/material';
import { ToolBar } from '../Miscelleneous';
import api from '../api/api';

// SQL datetime2 responses from older endpoints lack an offset; this app stores them in UTC.
export const asUtcDate = value => value ? new Date(/(?:Z|[+-]\d{2}:\d{2})$/i.test(value) ? value : value + 'Z') : null;
const displayDate = value => {
  const date = asUtcDate(value);
  return date && !Number.isNaN(date.getTime()) ? date.toLocaleString() : 'N/A';
};
const RecallStats = () => {
  const { recallId } = useParams();
  const navigate = useNavigate();
  const location = useLocation();
  const [recall, setRecall] = useState(null);
  const [contacts, setContacts] = useState([]);
  const [hasSnapshot, setHasSnapshot] = useState(false);
  const [rank, setRank] = useState('All');
  const [error, setError] = useState('');
  const [loaded, setLoaded] = useState(false);

  useEffect(() => {
    let cancelled = false;
    let timer;
    setLoaded(false);
    setRecall(null);
    setContacts([]);
    setError('');
    const refresh = async () => {
      try {
        const [details, snapshot] = await Promise.all([
          api.get(`/Recall/${recallId}`), api.get(`/Recall/${recallId}/recipients`)
        ]);
        if (cancelled) return;
        setRecall(details.data);
        setHasSnapshot(snapshot.data.hasRecipientSnapshot);
        setContacts(snapshot.data.recipients);
        setLoaded(true);
        setError('');
      } catch {
        if (!cancelled) setError('Recall data could not be refreshed. Previously loaded results may be out of date; a connection error does not mean someone has not responded.');
      } finally {
        if (!cancelled) timer = setTimeout(refresh, 5000);
      }
    };
    refresh();
    return () => { cancelled = true; clearTimeout(timer); };
  }, [recallId]);

  const filtered = contacts.filter(c => rank === 'All' || c.rank === rank);
  const acknowledged = filtered.filter(c => c.responded).length;
  const progress = filtered.length ? acknowledged / filtered.length * 100 : 0;
  const roles = ['All', ...new Set(contacts.map(c => c.rank))];

  return <div>
    <ToolBar />
    <Container component="main" id="main-content" maxWidth="md" sx={{ py: 4 }}>
      <Typography variant="h4" component="h1" gutterBottom>Recall progress</Typography>
      {location.state?.submitted && <Alert severity="success" sx={{ mb: 2 }}>Recall created and all messages submitted to the SMS provider. Submission is not confirmation of delivery.</Alert>}
      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
      {!loaded && !error && <Typography role="status">Loading recall…</Typography>}
      {loaded && <>
        <Paper variant="outlined" sx={{ p: 3, mb: 3 }}>
          <Typography variant="h6" component="h2" sx={{ overflowWrap: 'anywhere' }}>{recall.message}</Typography>
          <Typography sx={{ mt: 2 }}>Started: {displayDate(recall.timeStarted)}</Typography>
          <Typography>Deadline: {displayDate(recall.timeEnded)}</Typography>
        </Paper>
        {!hasSnapshot && <Alert severity="warning" sx={{ mb: 3 }}>Legacy recall: the original recipient list was not saved. Only recorded acknowledgments are shown; completion percentage and historical names/ranks are unavailable.</Alert>}
        <Tabs value={roles.includes(rank) ? rank : 'All'} onChange={(_, value) => setRank(value)} variant="scrollable" scrollButtons="auto" aria-label="Recipient rank">
          {roles.map(role => <Tab key={role} label={role} value={role} />)}
        </Tabs>
        {hasSnapshot && <Box sx={{ my: 3 }}>
          <Typography>{acknowledged} of {filtered.length} acknowledged ({Math.round(progress)}%)</Typography>
          <LinearProgress variant="determinate" value={progress} aria-label="Acknowledgment progress" sx={{ mt: 1, height: 8, borderRadius: 4 }} />
        </Box>}
        {!filtered.length && <Typography sx={{ my: 3 }}>No recipients or acknowledgments to display.</Typography>}
        {filtered.map(contact => {
          const responseTime = asUtcDate(contact.responseTime);
          const deadline = asUtcDate(recall.timeEnded);
          const lateMinutes = responseTime && deadline ? Math.max(0, Math.ceil((responseTime - deadline) / 60000)) : 0;
          return <Paper variant="outlined" key={contact.contactId} sx={{ my: 2, p: 2 }}>
            <Typography fontWeight={700}>{contact.firstName} {contact.lastName}</Typography>
            <Typography color="text.secondary">{contact.rank}</Typography>
            <Typography>{contact.responded ? 'Acknowledged · ' + displayDate(contact.responseTime) : 'Awaiting acknowledgment'}</Typography>
            {contact.responded && <Typography color={lateMinutes ? 'error' : 'success.main'}>{lateMinutes ? `${lateMinutes} minute(s) after deadline` : 'Responded within timeframe'}</Typography>}
          </Paper>;
        })}
      </>}
      <Button onClick={() => navigate('/landing')} sx={{ mt: 2 }}>Back to dashboard</Button>
    </Container>
  </div>;
};
export default RecallStats;
