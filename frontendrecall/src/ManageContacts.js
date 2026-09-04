import React, { useState, useEffect, useRef } from 'react';
import { Alert, Button, Stack, Tabs, Tab, TextField } from '@mui/material';
import { Link } from 'react-router-dom';
import useContacts from './hooks/UseContacts';
import api from './api/api';
import './css/ItemRows.css';

export const staffRanks = ['Employee', 'Element Chief', 'Flight Chief', 'Squadron Director'];

export const EditableRow = ({ item, onSave }) => {
  const [editing, setEditing] = useState(false);
  const [saved, setSaved] = useState(item);
  const [draft, setDraft] = useState(item);
  const [pending, setPending] = useState(false);
  const [error, setError] = useState('');
  const submitting = useRef(false);
  useEffect(() => { setSaved(item); }, [item]);
  const edit = () => { setDraft(saved); setError(''); setEditing(true); };
  const cancel = () => { setDraft(saved); setError(''); setEditing(false); };
  const save = async () => {
    if (submitting.current) return;
    if (!draft.firstName.trim() || !draft.lastName.trim() || !draft.phoneNumber.trim() || !staffRanks.includes(draft.rank)) {
      setError('Enter a first name, last name, phone number, and staff rank.');
      return;
    }
    submitting.current = true;
    setPending(true);
    setError('');
    try {
      const result = await onSave({ ...draft, firstName: draft.firstName.trim(), lastName: draft.lastName.trim(), phoneNumber: draft.phoneNumber.trim() });
      setSaved(result || draft);
      setEditing(false);
    } catch (failure) {
      setError(failure.response?.data?.message || 'Changes could not be saved. Your edits are still here; try again.');
    } finally { submitting.current = false; setPending(false); }
  };
  const input = (field, label) => <TextField size="small" label={label} value={draft[field] || ''} disabled={pending}
    onChange={event => setDraft({ ...draft, [field]: event.target.value })} sx={{ my: 0.5 }} />;
  return <>
    <tr>
      {editing ? <>
        <td>{input('firstName', 'First name')}{input('lastName', 'Last name')}</td>
        <td>{input('phoneNumber', 'Phone number')}</td>
        <td><select aria-label="Staff rank" value={draft.rank} disabled={pending} onChange={event => setDraft({ ...draft, rank: event.target.value })}>
          {staffRanks.map(rank => <option key={rank}>{rank}</option>)}
        </select></td>
        <td><Button onClick={save} disabled={pending}>{pending ? 'Saving…' : 'Save'}</Button><Button onClick={cancel} disabled={pending}>Cancel</Button></td>
      </> : <>
        <td>{saved.firstName} {saved.lastName}</td><td>{saved.phoneNumber}</td><td>{saved.rank}</td>
        <td><Button onClick={edit} aria-label={`Edit ${saved.firstName} ${saved.lastName}`}>Edit</Button></td>
      </>}
    </tr>
    {error && <tr><td colSpan={4}><Alert severity="error">{error}</Alert></td></tr>}
  </>;
};

const ManageContacts = () => {
  const { contacts, loading, error } = useContacts();
  const [updates, setUpdates] = useState({});
  const [rank, setRank] = useState('All');
  const [search, setSearch] = useState('');
  const [success, setSuccess] = useState('');
  const query = search.trim().toLowerCase();
  const filtered = contacts.map(contact => updates[contact.contactId] || contact).filter(contact =>
    (rank === 'All' || contact.rank === rank) &&
    `${contact.firstName} ${contact.lastName} ${contact.phoneNumber}`.toLowerCase().includes(query));
  const save = async contact => {
    setSuccess('');
    const response = await api.put(`/contact/${contact.contactId}`, contact);
    const saved = response.data && typeof response.data === 'object' ? response.data : contact;
    setUpdates(previous => ({ ...previous, [contact.contactId]: saved }));
    setSuccess('Contact updated.');
    return saved;
  };
  if (loading) return <p role="status">Loading staff…</p>;
  if (error) return <Alert severity="error">Staff could not be loaded. Please refresh and try again.</Alert>;
  return <section>
    <Stack direction="row" justifyContent="space-between" alignItems="center" sx={{ mb: 2 }}>
      <h2>Staff directory</h2><Button component={Link} to="/insertContact">Add a contact</Button>
    </Stack>
    {success && <Alert severity="success" sx={{ mb: 2 }}>{success}</Alert>}
    <TextField label="Search name or phone" value={search} onChange={event => setSearch(event.target.value)} fullWidth size="small" />
    <Tabs value={rank} onChange={(_, value) => setRank(value)} variant="scrollable" scrollButtons="auto" aria-label="Staff rank filter">
      {['All', ...staffRanks].map(value => <Tab key={value} value={value} label={value} />)}
    </Tabs>
    <div style={{ overflowX: 'auto' }}><table style={{ width: '100%', minWidth: 580 }}>
      <thead><tr><th scope="col">Name</th><th scope="col">Phone</th><th scope="col">Rank</th><th scope="col">Action</th></tr></thead>
      <tbody>{filtered.map(contact => <EditableRow key={contact.contactId} item={contact} onSave={save} />)}</tbody>
    </table></div>
    {!filtered.length && <p style={{ marginTop: 20 }}>{contacts.length ? 'No staff match these filters.' : 'No staff yet. Add a contact to get started.'}</p>}
  </section>;
};
export default ManageContacts;
