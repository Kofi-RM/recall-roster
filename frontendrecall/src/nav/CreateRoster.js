import React, { useState } from 'react';
import { Alert, Button, Checkbox, Container, FormControlLabel, Stack, TextField, Typography } from '@mui/material';
import { useNavigate } from 'react-router-dom';
import { ToolBar } from '../Miscelleneous';
import { UnitPicker, useUnitAccess } from '../UnitAccess';
import useContacts from '../hooks/UseContacts';
import api from '../api/api';

export default function CreateRoster() {
  const access = useUnitAccess();
  const navigate = useNavigate();
  const { contacts, loading, error } = useContacts();
  const [draft, setDraft] = useState({ unitId: '', name: '', description: '' });
  const [selected, setSelected] = useState([]);
  const [pending, setPending] = useState(false);
  const [failure, setFailure] = useState('');
  const [createdId, setCreatedId] = useState(null);
  const submit = async event => {
    event.preventDefault();
    if (pending || createdId || !access.canManage(draft.unitId)) return;
    setPending(true); setFailure('');
    let id;
    try {
      const response = await api.post('/roster', draft);
      id = response.data.rosterId; setCreatedId(id);
      await api.post('/RosterContact/updateContacts', { rosterId: id, contactsToAdd: selected, contactsToRemove: [] });
      navigate('/landing');
    } catch (failure) { setFailure(id ? `Roster #${id} was created, but membership could not be confirmed. Open it to check members before trying again.` : failure.response?.data?.message || 'Creation could not be confirmed. Check rosters before retrying.'); }
    finally { setPending(false); }
  };
  return <><ToolBar /><Container maxWidth="sm" sx={{ py: 4 }}>
    <Typography variant="h4" component="h1">Create roster</Typography>
    {(failure || error) && <Alert severity="error">{failure || 'Staff could not be loaded.'}</Alert>}
    <Stack component="form" onSubmit={submit} spacing={2} sx={{ mt: 3 }}>
      <UnitPicker value={draft.unitId} disabled={pending || !!createdId} onChange={unitId => { setDraft({ ...draft, unitId }); setSelected([]); }} />
      <TextField label="Roster name" required value={draft.name} disabled={pending || !!createdId} onChange={event => setDraft({ ...draft, name: event.target.value })} />
      <TextField label="Description" multiline value={draft.description} disabled={pending || !!createdId} onChange={event => setDraft({ ...draft, description: event.target.value })} />
      <Typography>Members must belong to the roster’s unit.</Typography>
      {contacts.filter(contact => contact.unitId === Number(draft.unitId)).map(contact => <FormControlLabel key={contact.contactId} label={`${contact.firstName} ${contact.lastName}`} control={<Checkbox checked={selected.includes(contact.contactId)} disabled={pending || !!createdId} onChange={() => setSelected(ids => ids.includes(contact.contactId) ? ids.filter(id => id !== contact.contactId) : [...ids, contact.contactId])} />} />)}
      <Button type="submit" variant="contained" disabled={pending || loading || !!error || !!createdId || !access.canManage(draft.unitId)}>{pending ? 'Creating…' : 'Create roster'}</Button>
      {createdId && <Button onClick={() => navigate(`/editRoster/${createdId}`)}>Check saved roster</Button>}
      <Button onClick={() => navigate('/landing')}>Back to dashboard</Button>
    </Stack>
  </Container></>;
}
