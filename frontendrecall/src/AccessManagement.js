import React, { useCallback, useEffect, useState } from 'react';
import { Alert, Button, Checkbox, Container, FormControlLabel, MenuItem, Paper, Stack, TextField, Typography } from '@mui/material';
import { Link } from 'react-router-dom';
import { ToolBar } from './Miscelleneous';
import { useUnitAccess } from './UnitAccess';
import api from './api/api';

export default function AccessManagement() {
  const access = useUnitAccess();
  const [data, setData] = useState({ users: [], units: [], memberships: [], audit: [] });
  const [unitNames, setUnitNames] = useState({});
  const [unit, setUnit] = useState({ name: '', parentUnitId: '' });
  const [account, setAccount] = useState({ email: '', password: '' });
  const [resetUserId, setResetUserId] = useState('');
  const [newPassword, setNewPassword] = useState('');
  const [userId, setUserId] = useState('');
  const [unitId, setUnitId] = useState('');
  const [grant, setGrant] = useState({ canManageRosters: false, canInitiateRecalls: false, includeDescendants: false });
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const load = useCallback(async () => {
    const [users, units, memberships, audit] = await Promise.all(['/User', '/access/units', '/access/memberships', '/access/audit'].map(url => api.get(url)));
    setData({ users: users.data, units: units.data, memberships: memberships.data, audit: audit.data });
  }, []);
  useEffect(() => {
    if (access.isAdministrator) load().catch(() => setError('Administration data could not be loaded.'));
    else setData({ users: [], units: [], memberships: [], audit: [] });
  }, [access.isAdministrator, load]);
  useEffect(() => {
    const membership = data.memberships.find(item => item.userId === Number(userId) && item.unitId === Number(unitId));
    setGrant({ canManageRosters: membership?.canManageRosters || false, canInitiateRecalls: membership?.canInitiateRecalls || false, includeDescendants: membership?.includeDescendants || false });
  }, [data.memberships, userId, unitId]);
  const run = async (action, message) => {
    if (busy) return;
    setBusy(true); setError(''); setSuccess('');
    try { await action(); await load(); access.refresh(); setSuccess(message); }
    catch (failure) { setError(failure.response?.data?.message || 'The change could not be confirmed. Refresh before retrying.'); }
    finally { setBusy(false); }
  };
  if (access.loading) return <p>Loading permissions…</p>;
  if (!access.isAdministrator) return <Container><Alert severity="info">Administrator access is required.</Alert><Button component={Link} to="/landing">Back to dashboard</Button></Container>;
  const name = id => data.units.find(item => item.id === id)?.name || `Unit #${id}`;
  const email = id => data.users.find(item => item.id === id)?.email || `User #${id}`;
  return <><ToolBar /><Container maxWidth="md" sx={{ py: 4 }}>
    <Typography variant="h4" component="h1">Units and access</Typography>
    <Typography sx={{ mb: 2 }}>Unit membership grants viewing. Management, recall initiation, and child-unit access are separate permissions. Administrator status alone does not grant operational access.</Typography>
    <Button component={Link} to="/landing">Back to dashboard</Button>
    {error && <Alert severity="error">{error}</Alert>}{success && <Alert severity="success">{success}</Alert>}
    <Paper variant="outlined" sx={{ p: 3, my: 2 }}><Typography variant="h6" component="h2">Create unit</Typography>
      <Stack component="form" spacing={2} onSubmit={event => { event.preventDefault(); run(() => api.post('/access/units', { name: unit.name, parentUnitId: unit.parentUnitId === '' ? null : Number(unit.parentUnitId) }), 'Unit created.'); }}>
        <TextField label="Unit name" required inputProps={{ maxLength: 160 }} value={unit.name} onChange={event => setUnit({ ...unit, name: event.target.value })} />
        <TextField select label="Parent unit" value={unit.parentUnitId} onChange={event => setUnit({ ...unit, parentUnitId: event.target.value })}><MenuItem value="">No parent</MenuItem>{data.units.map(item => <MenuItem key={item.id} value={item.id}>{item.name}</MenuItem>)}</TextField>
        <Button type="submit" disabled={busy}>Create unit</Button>
      </Stack>
    </Paper>
    <Paper variant="outlined" sx={{ p: 3, my: 2 }}><Typography variant="h6" component="h2">Unit directory</Typography>
      <Typography>Names can change without changing ownership or permissions. Parent relationships stay fixed to avoid silently expanding access.</Typography>
      {data.units.map(item => <Stack key={item.id} direction="row" spacing={2} sx={{ mt: 2 }}>
        <TextField label={`Name for unit ${item.id}`} value={unitNames[item.id] ?? item.name} onChange={event => setUnitNames({ ...unitNames, [item.id]: event.target.value })} inputProps={{ maxLength: 160 }} />
        <Button disabled={busy || !(unitNames[item.id] ?? item.name).trim()} onClick={() => run(() => api.put(`/access/units/${item.id}`, { name: unitNames[item.id] ?? item.name }), 'Unit name updated.')}>Save name</Button>
      </Stack>)}
    </Paper>
    <Paper variant="outlined" sx={{ p: 3, my: 2 }}><Typography variant="h6" component="h2">Create coordinator or viewer account</Typography>
      <Stack component="form" spacing={2} onSubmit={event => { event.preventDefault(); run(async () => { await api.post('/auth/register', account); setAccount({ email: '', password: '' }); }, 'Account created without unit permissions. Assign access below.'); }}>
        <TextField label="Account email" type="email" required value={account.email} onChange={event => setAccount({ ...account, email: event.target.value })} />
        <TextField label="Initial password" type="password" autoComplete="new-password" required inputProps={{ minLength: 12, maxLength: 128 }} value={account.password} onChange={event => setAccount({ ...account, password: event.target.value })} />
        <Button type="submit" disabled={busy}>Create account</Button>
      </Stack>
    </Paper>
    <Paper variant="outlined" sx={{ p: 3, my: 2 }}><Typography variant="h6" component="h2">Assign unit permissions</Typography>
      <Stack spacing={2}>
        <TextField select label="Account" value={userId} onChange={event => setUserId(event.target.value)}>{data.users.filter(user => user.isEnabled).map(user => <MenuItem key={user.id} value={user.id}>{user.email}</MenuItem>)}</TextField>
        <TextField select label="Assigned unit" value={unitId} onChange={event => setUnitId(event.target.value)}>{data.units.map(item => <MenuItem key={item.id} value={item.id}>{item.name}</MenuItem>)}</TextField>
        <Typography>Saving with no boxes checked grants read-only access to this unit.</Typography>
        {[['canManageRosters', 'Manage staff and rosters'], ['canInitiateRecalls', 'Initiate recalls and send recall messages'], ['includeDescendants', 'Apply these permissions to child units']].map(([key, label]) => <FormControlLabel key={key} label={label} control={<Checkbox checked={grant[key]} onChange={event => setGrant({ ...grant, [key]: event.target.checked })} />} />)}
        <Button disabled={busy || !userId || !unitId} onClick={() => run(() => api.put(`/access/memberships/${userId}/${unitId}`, grant), 'Unit permissions saved.')}>Save permissions</Button>
      </Stack>
      {data.memberships.map(item => <Stack key={`${item.userId}-${item.unitId}`} direction="row" justifyContent="space-between" alignItems="center" sx={{ mt: 2 }}>
        <Typography>{email(item.userId)} · {name(item.unitId)} · {item.canManageRosters ? 'Manage' : 'View'}{item.canInitiateRecalls ? ' · Initiate' : ''}{item.includeDescendants ? ' · Includes child units' : ''}</Typography>
        <Button color="error" disabled={busy} onClick={() => run(() => api.delete(`/access/memberships/${item.userId}/${item.unitId}`), 'Membership revoked. Other overlapping grants may still provide access.')}>Revoke</Button>
      </Stack>)}
    </Paper>
    <Paper variant="outlined" sx={{ p: 3, my: 2 }}><Typography variant="h6" component="h2">Account status</Typography>
      {data.users.map(user => <Stack key={user.id} spacing={1} sx={{ my: 2 }}><Typography>{user.email} · {user.isEnabled ? 'Enabled' : 'Disabled'} · {user.isSystemAdministrator ? 'Administrator' : 'Unit member'}</Typography>
        <Stack direction="row"><Button disabled={busy} onClick={() => run(() => api.put(`/access/users/${user.id}`, { isEnabled: !user.isEnabled, isSystemAdministrator: user.isSystemAdministrator }), 'Account status updated.')}>{user.isEnabled ? 'Disable account' : 'Enable account'}</Button>
        <Button disabled={busy} onClick={() => run(() => api.put(`/access/users/${user.id}`, { isEnabled: user.isEnabled, isSystemAdministrator: !user.isSystemAdministrator }), 'Administrator permissions updated.')}>{user.isSystemAdministrator ? 'Remove administrator' : 'Make administrator'}</Button></Stack>
      </Stack>)}
    </Paper>
    <Paper variant="outlined" sx={{ p: 3, my: 2 }}><Typography variant="h6" component="h2">Reset account password</Typography>
      <Typography>Resetting a password does not enable a disabled account. Enable it separately after reviewing its unit access.</Typography>
      <Stack component="form" spacing={2} sx={{ mt: 2 }} onSubmit={event => { event.preventDefault(); run(async () => { await api.put(`/access/users/${resetUserId}/password`, { password: newPassword }); setNewPassword(''); }, 'Password reset.'); }}>
        <TextField select label="Account to reset" required value={resetUserId} onChange={event => setResetUserId(event.target.value)}>{data.users.map(user => <MenuItem key={user.id} value={user.id}>{user.email}</MenuItem>)}</TextField>
        <TextField type="password" label="New password" autoComplete="new-password" required inputProps={{ minLength: 12, maxLength: 128 }} value={newPassword} onChange={event => setNewPassword(event.target.value)} />
        <Button type="submit" disabled={busy || !resetUserId}>Reset password</Button>
      </Stack>
    </Paper>
    <Paper variant="outlined" sx={{ p: 3, my: 2 }}><Typography variant="h6" component="h2">Recent access changes</Typography>
      {data.audit.map(item => <Typography key={item.id} sx={{ my: 1 }}>{new Date(item.occurredAt.endsWith('Z') ? item.occurredAt : item.occurredAt + 'Z').toLocaleString()} · {item.action} · Actor #{item.actorUserId ?? 'system'} · {item.details}</Typography>)}
    </Paper>
  </Container></>;
}
