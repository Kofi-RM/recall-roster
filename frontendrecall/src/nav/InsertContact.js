import React, { useState } from 'react';
import { Alert, Button, Container, MenuItem, Stack, TextField, Typography } from '@mui/material';
import { useNavigate } from 'react-router-dom';
import { ToolBar } from '../Miscelleneous';
import { UnitPicker, useUnitAccess } from '../UnitAccess';
import { staffRanks } from '../ManageContacts';
import api from '../api/api';

export default function InsertContact() {
  const navigate = useNavigate();
  const access = useUnitAccess();
  const [draft, setDraft] = useState({ unitId: '', firstName: '', lastName: '', phoneNumber: '', rank: 'Employee' });
  const [pending, setPending] = useState(false);
  const [error, setError] = useState('');
  const save = async event => {
    event.preventDefault();
    if (pending || !access.canManage(draft.unitId)) return;
    setPending(true); setError('');
    try { await api.post('/Contact', draft); navigate('/landing'); }
    catch (failure) { setError(failure.response?.data?.message || 'Contact could not be added.'); }
    finally { setPending(false); }
  };
  return <><ToolBar /><Container maxWidth="sm" sx={{ py: 4 }}>
    <Typography variant="h4" component="h1">Add staff contact</Typography>
    {error && <Alert severity="error">{error}</Alert>}
    <Stack component="form" onSubmit={save} spacing={2} sx={{ mt: 3 }}>
      <UnitPicker value={draft.unitId} onChange={unitId => setDraft({ ...draft, unitId })} disabled={pending} />
      {[['firstName', 'First name'], ['lastName', 'Last name'], ['phoneNumber', 'Phone number']].map(([key, label]) => <TextField key={key} label={label} value={draft[key]} required disabled={pending} onChange={event => setDraft({ ...draft, [key]: event.target.value })} />)}
      <TextField select label="Staff rank" value={draft.rank} disabled={pending} onChange={event => setDraft({ ...draft, rank: event.target.value })}>{staffRanks.map(rank => <MenuItem key={rank} value={rank}>{rank}</MenuItem>)}</TextField>
      <Button type="submit" variant="contained" disabled={pending || !access.canManage(draft.unitId)}>{pending ? 'Saving…' : 'Add contact'}</Button>
      <Button onClick={() => navigate('/landing')}>Back to dashboard</Button>
    </Stack>
  </Container></>;
}
