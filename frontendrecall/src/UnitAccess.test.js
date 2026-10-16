import React from 'react';
import { act, fireEvent, render, screen, waitFor } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import { UnitAccessProvider, useUnitAccess } from './UnitAccess';
import AccessManagement from './AccessManagement';
import api from './api/api';
let mockToken = 'first-login';
jest.mock('./Auth', () => ({ useAuth: () => ({ token: mockToken }) }));
jest.mock('./api/api', () => ({ get: jest.fn(), put: jest.fn(), post: jest.fn(), delete: jest.fn() }));
jest.mock('./Miscelleneous', () => ({ ToolBar: () => null }));
const Probe = () => { const access = useUnitAccess(); return <div>{access.loading ? 'Loading access' : access.units.map(unit => unit.name).join(', ') || 'No units'}{access.error && <span>{access.error}</span>}<span>{access.isAdministrator ? 'Administrator' : 'Unit member'}</span></div>; };
const view = child => <UnitAccessProvider><MemoryRouter>{child}</MemoryRouter></UnitAccessProvider>;
beforeEach(() => { mockToken = 'first-login'; jest.clearAllMocks(); });
afterEach(() => jest.useRealTimers());

test('unit access is loaded from server and cleared when another user signs in', async () => {
  let finishSecond;
  api.get.mockResolvedValueOnce({ data: { units: [{ id: 1, name: 'Squadron A' }], isAdministrator: true } });
  const page = render(view(<Probe />));
  expect(await screen.findByText('Squadron A')).toBeInTheDocument();
  api.get.mockImplementationOnce(() => new Promise(resolve => { finishSecond = resolve; }));
  mockToken = 'second-login'; page.rerender(view(<Probe />));
  expect(screen.queryByText('Squadron A')).not.toBeInTheDocument();
  expect(screen.queryByText('Administrator')).not.toBeInTheDocument();
  await act(async () => finishSecond({ data: { units: [], isAdministrator: false } }));
  expect(await screen.findByText('No units')).toBeInTheDocument();
});

test('permission refresh failure removes cached privileges', async () => {
  api.get.mockResolvedValueOnce({ data: { units: [{ id: 1, name: 'Squadron A' }], isAdministrator: true } });
  render(view(<Probe />)); await screen.findByText('Squadron A');
  api.get.mockRejectedValueOnce(new Error('Disconnected'));
  await act(async () => { fireEvent(window, new Event('focus')); });
  expect(await screen.findByText(/Unit access could not be loaded/)).toBeInTheDocument();
  expect(screen.queryByText('Administrator')).not.toBeInTheDocument();
  expect(screen.queryByText('Squadron A')).not.toBeInTheDocument();
});

test('unit members cannot open administrator management or fetch account directory', async () => {
  api.get.mockResolvedValue({ data: { units: [{ id: 1, name: 'Flight' }], isAdministrator: false } });
  await act(async () => { render(view(<AccessManagement />)); });
  expect(await screen.findByText('Administrator access is required.')).toBeInTheDocument();
  expect(api.get.mock.calls.map(([url]) => url)).toEqual(['/access/me']);
});

test('administrator can explicitly assign read-only membership without write permissions', async () => {
  api.get.mockImplementation(url => Promise.resolve({ data: url === '/access/me' ? { units: [], isAdministrator: true }
    : url === '/User' ? [{ id: 7, email: 'viewer@example.test', isEnabled: true }]
    : url === '/access/units' ? [{ id: 2, name: 'Flight A' }] : [] }));
  api.put.mockResolvedValue({ data: {} });
  await act(async () => { render(view(<AccessManagement />)); });
  await screen.findByRole('heading', { name: 'Assign unit permissions' });
  fireEvent.mouseDown(screen.getByLabelText('Account'));
  fireEvent.click(await screen.findByRole('option', { name: 'viewer@example.test' }));
  fireEvent.mouseDown(screen.getByLabelText('Assigned unit'));
  fireEvent.click(await screen.findByRole('option', { name: 'Flight A' }));
  await act(async () => { fireEvent.click(screen.getByRole('button', { name: 'Save permissions' })); });
  await waitFor(() => expect(api.put).toHaveBeenCalledWith('/access/memberships/7/2', { canManageRosters: false, canInitiateRecalls: false, includeDescendants: false }));
  expect(await screen.findByText('Unit permissions saved.')).toBeInTheDocument();
});
