import React, { useEffect, useState } from 'react';
import { Alert, Button, Checkbox, Container, FormControlLabel, Stack, TextField, Typography } from '@mui/material';
import { useNavigate, useParams } from 'react-router-dom';
import { ToolBar } from '../Miscelleneous';
import { useUnitAccess } from '../UnitAccess';
import api from '../api/api';

export default function EditRoster() {
  const { rosterId } = useParams();
  const navigate = useNavigate();
  const access = useUnitAccess();
  const [roster, setRoster] = useState(null);
  const [contacts, setContacts] = useState([]);
  const [initial, setInitial] = useState([]);
  const [selected, setSelected] = useState([]);
  const [error, setError] = useState('');
  const [pending, setPending] = useState(false);
  useEffect(() => {
    let cancelled = false;
    const load = async () => {
      try {
        const [details, members, staff] = await Promise.all([api.get(`/roster/${rosterId}`), api.get(`/rostercontact/${rosterId}`), api.get('/contact')]);
        if (cancelled) return;
        setRoster(details.data); setContacts(staff.data.filter(contact => contact.unitId === details.data.unitId));
        const ids = members.data.map(member => member.contactId); setInitial(ids); setSelected(ids);
      } catch { if (!cancelled) setError('This roster could not be loaded or is outside your unit access.'); }
    };
    load(); return () => { cancelled = true; };
  }, [rosterId]);
  const editable = roster && access.canManage(roster.unitId);
  const save = async event => {
    event.preventDefault();
    if (pending || !editable) return;
    setPending(true); setError('');
    try {
      await api.put(`/roster/${rosterId}`, roster);
      await api.post('/RosterContact/updateContacts', { rosterId: Number(rosterId), contactsToAdd: selected.filter(id => !initial.includes(id)), contactsToRemove: initial.filter(id => !selected.includes(id)) });
      navigate('/landing');
    } catch (failure) { setError(failure.response?.data?.message || 'Some changes may have saved. Reload the roster to check before retrying.'); }
    finally { setPending(false); }
  };
  return <><ToolBar /><Container maxWidth="sm" sx={{ py: 4 }}><Typography variant="h4" component="h1">{editable ? 'Edit roster' : 'Roster members'}</Typography>
    {error && <Alert severity="error">{error}</Alert>}
    {roster && <Stack component="form" spacing={2} onSubmit={save} sx={{ mt: 3 }}>
      <Typography>Unit: {access.unitName(roster.unitId)}</Typography>
      {!editable && <Alert severity="info">You have read-only access to this roster.</Alert>}
      <TextField label="Roster name" required value={roster.name} disabled={!editable || pending} onChange={event => setRoster({ ...roster, name: event.target.value })} />
      <TextField label="Description" value={roster.description || ''} disabled={!editable || pending} onChange={event => setRoster({ ...roster, description: event.target.value })} />
      {contacts.map(contact => <FormControlLabel key={contact.contactId} label={`${contact.firstName} ${contact.lastName}`} control={<Checkbox checked={selected.includes(contact.contactId)} disabled={!editable || pending} onChange={() => setSelected(ids => ids.includes(contact.contactId) ? ids.filter(id => id !== contact.contactId) : [...ids, contact.contactId])} />} />)}
      {editable && <Button type="submit" variant="contained" disabled={pending}>{pending ? 'Saving…' : 'Save roster'}</Button>}
    </Stack>}
    <Button onClick={() => navigate('/landing')} sx={{ mt: 2 }}>Back to dashboard</Button>
  </Container></>;
}
