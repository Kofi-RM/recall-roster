import React from 'react';
import { act, fireEvent, render, screen, within } from '@testing-library/react';
import { MemoryRouter, Route, Routes, useLocation } from 'react-router-dom';
import { AuthProvider, decodeSession, useAuth } from './Auth';
import ProtectedRoute from './ProtectedRoute';
import { EditableRow } from './ManageContacts';
import ManageRoster from './ManageRoster';
import api from './api/api';

jest.mock('./api/api', () => ({ put: jest.fn(), delete: jest.fn() }));
jest.mock('./hooks/UseRoster', () => () => ({ rosters: [{ rosterId: 1, name: 'Team', description: 'Test team' }], loading: false, error: null }));
const contact = { contactId: 1, firstName: 'First', lastName: 'Person', phoneNumber: '2025550100', rank: 'Employee', active: 1 };
const token = exp => 'eyJhbGciOiJub25lIn0.' + btoa(JSON.stringify({ exp, sub: '1' })) + '.test';
const renderRow = save => render(<table><tbody><EditableRow item={contact} onSave={save} /></tbody></table>);
beforeEach(() => { localStorage.clear(); jest.clearAllMocks(); });
afterEach(() => { jest.useRealTimers(); });

test('cancel discards staff edits', () => {
  renderRow(jest.fn());
  fireEvent.click(screen.getByRole('button', { name: 'Edit First Person' }));
  fireEvent.change(screen.getByLabelText('First name'), { target: { value: 'Unsaved' } });
  fireEvent.click(screen.getByRole('button', { name: 'Cancel' }));
  expect(screen.getByText('First Person')).toBeInTheDocument();
  fireEvent.click(screen.getByRole('button', { name: 'Edit First Person' }));
  expect(screen.getByLabelText('First name')).toHaveValue('First');
});

test('failed save retains edits and allows retry', async () => {
  const save = jest.fn().mockRejectedValueOnce(new Error('Offline')).mockImplementationOnce(value => Promise.resolve(value));
  renderRow(save);
  fireEvent.click(screen.getByRole('button', { name: 'Edit First Person' }));
  fireEvent.change(screen.getByLabelText('First name'), { target: { value: 'Updated' } });
  fireEvent.click(screen.getByRole('button', { name: 'Save' }));
  expect(await screen.findByRole('alert')).toHaveTextContent('Changes could not be saved');
  expect(screen.getByLabelText('First name')).toHaveValue('Updated');
  fireEvent.click(screen.getByRole('button', { name: 'Save' }));
  expect(await screen.findByText('Updated Person')).toBeInTheDocument();
});

test('roster removal requires confirmation and updates the list only after success', async () => {
  api.delete.mockResolvedValueOnce({});
  render(<MemoryRouter><ManageRoster /></MemoryRouter>);
  fireEvent.click(screen.getByRole('button', { name: 'Remove' }));
  expect(api.delete).not.toHaveBeenCalled();
  fireEvent.click(within(screen.getByRole('dialog')).getByRole('button', { name: 'Cancel' }));
  expect(api.delete).not.toHaveBeenCalled();
  fireEvent.click(await screen.findByRole('button', { name: 'Remove' }));
  fireEvent.click(within(screen.getByRole('dialog')).getByRole('button', { name: 'Remove roster' }));
  expect(await screen.findByText('No rosters yet. Create one to organize your team.')).toBeInTheDocument();
  expect(api.delete).toHaveBeenCalledWith('/roster/remove/1');
});

test('failed roster removal keeps the row and explains failure', async () => {
  api.delete.mockRejectedValueOnce(new Error('Offline'));
  render(<MemoryRouter><ManageRoster /></MemoryRouter>);
  fireEvent.click(screen.getByRole('button', { name: 'Remove' }));
  fireEvent.click(within(screen.getByRole('dialog')).getByRole('button', { name: 'Remove roster' }));
  expect(await screen.findByRole('alert')).toHaveTextContent('The roster could not be removed');
  fireEvent.click(within(screen.getByRole('dialog')).getByRole('button', { name: 'Cancel' }));
  expect(await screen.findByRole('heading', { name: 'Team' })).toBeInTheDocument();
});

test('expired and malformed tokens are rejected', () => {
  expect(decodeSession(token(Date.now() / 1000 - 1))).toBeNull();
  expect(decodeSession('invalid')).toBeNull();
  localStorage.setItem('recallToken', 'invalid');
  render(<AuthProvider><MemoryRouter initialEntries={['/protected']}><Routes>
    <Route path="/protected" element={<ProtectedRoute><p>Private content</p></ProtectedRoute>} />
    <Route path="/login" element={<p>Login destination</p>} />
  </Routes></MemoryRouter></AuthProvider>);
  expect(screen.getByText('Login destination')).toBeInTheDocument();
  expect(localStorage.getItem('recallToken')).toBeNull();
});

test('valid stored sessions are immediately available and expire automatically', () => {
  jest.useFakeTimers();
  localStorage.setItem('recallToken', token(Math.floor(Date.now() / 1000) + 2));
  const Probe = () => <p>{useAuth().isLoggedIn ? 'Signed in' : 'Signed out'}</p>;
  render(<AuthProvider><Probe /></AuthProvider>);
  expect(screen.getByText('Signed in')).toBeInTheDocument();
  act(() => jest.advanceTimersByTime(2100));
  expect(screen.getByText('Signed out')).toBeInTheDocument();
  expect(localStorage.getItem('recallToken')).toBeNull();
});

test('logout in another tab clears the current session', () => {
  localStorage.setItem('recallToken', token(Math.floor(Date.now() / 1000) + 1000));
  const Probe = () => <p>{useAuth().isLoggedIn ? 'Signed in' : 'Signed out'}</p>;
  render(<AuthProvider><Probe /></AuthProvider>);
  act(() => { localStorage.removeItem('recallToken'); window.dispatchEvent(new StorageEvent('storage', { key: 'recallToken' })); });
  expect(screen.getByText('Signed out')).toBeInTheDocument();
});

test('protected routes preserve the requested destination for sign-in', () => {
  const LoginDestination = () => <p>{useLocation().state.from}</p>;
  render(<AuthProvider><MemoryRouter initialEntries={['/protected?filter=active#row']}><Routes>
    <Route path="/protected" element={<ProtectedRoute><p>Private content</p></ProtectedRoute>} />
    <Route path="/login" element={<LoginDestination />} />
  </Routes></MemoryRouter></AuthProvider>);
  expect(screen.getByText('/protected?filter=active#row')).toBeInTheDocument();
});
