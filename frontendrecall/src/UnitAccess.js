import React, { createContext, useCallback, useContext, useEffect, useState } from 'react';
import { Alert, MenuItem, TextField } from '@mui/material';
import { useAuth } from './Auth';
import api from './api/api';

const empty = { units: [], isAdministrator: false, loading: false, error: '', canManage: () => false, canInitiate: () => false, unitName: () => '', refresh: async () => {} };
const UnitAccessContext = createContext(empty);
export const useUnitAccess = () => useContext(UnitAccessContext);
export const UnitAccessProvider = ({ children }) => {
  const { token } = useAuth();
  const [state, setState] = useState({ token: null, units: [], isAdministrator: false, loading: false, error: '' });
  const [revision, setRevision] = useState(0);
  const refresh = useCallback(() => setRevision(value => value + 1), []);
  useEffect(() => {
    if (!token) return;
    let cancelled = false;
    let timer;
    const load = async () => {
      try {
        const response = await api.get('/access/me');
        if (!cancelled) setState({ ...response.data, token, loading: false, error: '' });
      } catch {
        // Fail closed and remove cached privileges when current access cannot be confirmed.
        if (!cancelled) setState({ token, units: [], isAdministrator: false, loading: false, error: 'Unit access could not be loaded. Try refreshing access.' });
      } finally { if (!cancelled) timer = setTimeout(load, 30000); }
    };
    load();
    const onFocus = () => { clearTimeout(timer); load(); };
    window.addEventListener('focus', onFocus);
    return () => { cancelled = true; clearTimeout(timer); window.removeEventListener('focus', onFocus); };
  }, [token, revision]);
  const current = token && state.token === token ? state : { ...empty, loading: !!token };
  const value = { ...current, refresh,
    canManage: id => current.units.some(unit => unit.id === Number(id) && unit.canManageRosters),
    canInitiate: id => current.units.some(unit => unit.id === Number(id) && unit.canInitiateRecalls),
    unitName: id => current.units.find(unit => unit.id === Number(id))?.name || 'Unit #' + id
  };
  return <UnitAccessContext.Provider value={value}>{children}</UnitAccessContext.Provider>;
};

export const UnitPicker = ({ value, onChange, permission = 'canManageRosters', disabled = false }) => {
  const access = useUnitAccess();
  const units = access.units.filter(unit => unit[permission]);
  return <>
    {access.error && <Alert severity="error">{access.error}</Alert>}
    {!access.loading && !units.length && <Alert severity="info">An administrator must grant you the required unit permission.</Alert>}
    <TextField select label="Unit" value={value} onChange={event => onChange(Number(event.target.value))} required fullWidth disabled={disabled || access.loading || !units.length}>
      {units.map(unit => <MenuItem key={unit.id} value={unit.id}>{unit.name}</MenuItem>)}
    </TextField>
  </>;
};
