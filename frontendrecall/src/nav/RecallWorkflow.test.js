import React from 'react';
import { fireEvent, render, screen, waitFor } from '@testing-library/react';
import { MemoryRouter, Route, Routes } from 'react-router-dom';
import RecallStats, { asUtcDate } from './RecallStats';
import StartRecall from './StartRecall';
import api from '../api/api';

jest.mock('../api/api', () => ({ get: jest.fn(), post: jest.fn() }));
jest.mock('../Miscelleneous', () => ({ ToolBar: () => null }));
jest.mock('../hooks/UseRoster', () => () => ({ rosters: [{ rosterId: 1, name: 'Team' }], loading: false, error: null }));
beforeEach(() => jest.clearAllMocks());
const details = { message: 'Report in', timeStarted: '2026-09-04T12:00:00', timeEnded: '2026-09-04T13:00:00' };
const renderStats = () => render(<MemoryRouter initialEntries={['/recallStats/7']}><Routes><Route path="/recallStats/:recallId" element={<RecallStats />} /></Routes></MemoryRouter>);

test('interprets both legacy datetime2 and offset-bearing timestamps correctly', () => {
  expect(asUtcDate('2026-09-04T12:00:00').toISOString()).toBe('2026-09-04T12:00:00.000Z');
  expect(asUtcDate('2026-09-04T12:00:00Z').toISOString()).toBe('2026-09-04T12:00:00.000Z');
  expect(asUtcDate('2026-09-04T08:00:00-04:00').toISOString()).toBe('2026-09-04T12:00:00.000Z');
});

test('progress uses the snapshot endpoint, not live roster members', async () => {
  api.get.mockImplementation(url => Promise.resolve({ data: url.endsWith('/recipients') ? {
    hasRecipientSnapshot: true, recipients: [
      { contactId: 1, firstName: 'Original', lastName: 'Person', rank: 'Employee', responded: true, responseTime: '2026-09-04T12:30:00Z' },
      { contactId: 2, firstName: 'Waiting', lastName: 'Person', rank: 'Employee', responded: false }
    ]
  } : details }));
  renderStats();
  expect(await screen.findByText('1 of 2 acknowledged (50%)')).toBeInTheDocument();
  expect(screen.getByText('Original Person')).toBeInTheDocument();
  expect(api.get.mock.calls.map(([url]) => url)).toEqual(['/Recall/7', '/Recall/7/recipients']);
});

test('legacy recalls explain the missing snapshot and do not show a percentage', async () => {
  api.get.mockImplementation(url => Promise.resolve({ data: url.endsWith('/recipients') ? {
    hasRecipientSnapshot: false, recipients: []
  } : details }));
  renderStats();
  expect(await screen.findByText(/Legacy recall:/)).toBeInTheDocument();
  expect(screen.queryByRole('progressbar')).not.toBeInTheDocument();
});

const fillRecall = async () => {
  render(<MemoryRouter><StartRecall /></MemoryRouter>);
  fireEvent.mouseDown(screen.getByLabelText(/roster/i));
  fireEvent.click(await screen.findByRole('option', { name: 'Team' }));
  fireEvent.change(screen.getByLabelText(/message/i), { target: { value: 'Report in' } });
  fireEvent.click(screen.getByRole('button', { name: 'Start recall' }));
};

test('failed recall creation never sends messages or displays success', async () => {
  api.post.mockRejectedValueOnce(new Error('Unavailable'));
  await fillRecall();
  expect(await screen.findByRole('alert')).toHaveTextContent('Recall creation could not be confirmed');
  expect(api.post).toHaveBeenCalledTimes(1);
  expect(api.get).not.toHaveBeenCalled();
});

test('partial SMS failure preserves the created recall and prevents duplicate creation', async () => {
  api.post.mockResolvedValueOnce({ data: { recallId: 7 } }).mockRejectedValueOnce(new Error('Provider unavailable'));
  api.get.mockResolvedValueOnce({ data: { recipients: [{ contactId: 1 }] } });
  await fillRecall();
  expect(await screen.findByRole('alert')).toHaveTextContent('Recall #7 was created, but 1 message submission(s) failed');
  await waitFor(() => expect(screen.getByRole('button', { name: 'Start recall' })).toBeDisabled());
  expect(screen.getByRole('link', { name: 'View recall #7' })).toHaveAttribute('href', '/recallStats/7');
  expect(api.post).toHaveBeenNthCalledWith(2, '/Message/SendMessage/1/7', { message: 'Report in' });
});
