import React, { useRef, useState } from 'react';
import { Alert, Button, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle, Paper, Stack, TextField, Typography } from '@mui/material';
import { Link } from 'react-router-dom';
import useRoster from './hooks/UseRoster';
import api from './api/api';

export const RemoveContact = ({ children }) => <span className="button">{children}</span>;
const ManageRoster = () => {
  const { rosters, loading, error } = useRoster();
  const [removed, setRemoved] = useState([]);
  const [selected, setSelected] = useState(null);
  const [pending, setPending] = useState(false);
  const [failure, setFailure] = useState('');
  const [success, setSuccess] = useState('');
  const [search, setSearch] = useState('');
  const submitting = useRef(false);
  const visible = rosters.filter(roster => !removed.includes(roster.rosterId) &&
    `${roster.name} ${roster.description || ''}`.toLowerCase().includes(search.trim().toLowerCase()));
  const remove = async () => {
    if (!selected || submitting.current) return;
    submitting.current = true;
    setPending(true);
    setFailure('');
    try {
      try { await api.delete(`/roster/remove/${selected.rosterId}`); }
      catch (err) { if (err.response?.status !== 404) throw err; }
      setRemoved(previous => [...previous, selected.rosterId]);
      setSuccess(`“${selected.name}” was removed. Saved recall snapshots are unchanged.`);
      setSelected(null);
    } catch {
      setFailure('The roster could not be removed. Please try again.');
    } finally { submitting.current = false; setPending(false); }
  };
  if (loading) return <p role="status">Loading rosters…</p>;
  if (error) return <Alert severity="error">Rosters could not be loaded. Please refresh and try again.</Alert>;
  return <section>
    <Stack direction="row" alignItems="center" justifyContent="space-between" sx={{ mb: 2 }}>
      <Typography variant="h5" component="h2">Your rosters</Typography>
      <Button component={Link} to="/createRoster" variant="contained">Create roster</Button>
    </Stack>
    {success && <Alert severity="success" sx={{ mb: 2 }}>{success}</Alert>}
    <TextField label="Search rosters" value={search} onChange={event => setSearch(event.target.value)} fullWidth size="small" sx={{ mb: 2 }} />
    {visible.map(roster => <Paper variant="outlined" key={roster.rosterId} sx={{ p: 2, mb: 2 }}>
      <Typography component="h3" variant="h6">{roster.name}</Typography>
      <Typography color="text.secondary" sx={{ mb: 1 }}>{roster.description}</Typography>
      <Button component={Link} to={`/editRoster/${roster.rosterId}`}>Edit</Button>
      <Button color="error" onClick={() => { setSelected(roster); setFailure(''); setSuccess(''); }}>Remove</Button>
    </Paper>)}
    {!visible.length && <p>{search ? 'No rosters match your search.' : 'No rosters yet. Create one to organize your team.'}</p>}
    <Dialog open={!!selected} onClose={() => { if (!pending) setSelected(null); }} aria-labelledby="remove-roster-title">
      <DialogTitle id="remove-roster-title">Remove {selected?.name}?</DialogTitle>
      <DialogContent>
        <DialogContentText>This removes the roster and its memberships, not the staff contacts or saved recall snapshots. This cannot be undone here.</DialogContentText>
        {failure && <Alert severity="error" sx={{ mt: 2 }}>{failure}</Alert>}
      </DialogContent>
      <DialogActions><Button onClick={() => setSelected(null)} disabled={pending}>Cancel</Button>
        <Button color="error" onClick={remove} disabled={pending}>{pending ? 'Removing…' : 'Remove roster'}</Button></DialogActions>
    </Dialog>
  </section>;
};
export default ManageRoster;
