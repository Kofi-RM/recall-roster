import { fireEvent, render, screen, waitFor } from '@testing-library/react';
import Demo from './Demo';

const respond = (data, ok = true, status = 200) => Promise.resolve({ ok, status, json: () => Promise.resolve(data) });
beforeEach(() => { sessionStorage.clear(); global.fetch = jest.fn(); });
afterEach(() => { jest.restoreAllMocks(); });

test('requires consent and phone verification, then disables sending after two attempts', async () => {
  fetch.mockImplementationOnce(() => respond({ token: 'session', message: 'Code sent' }))
    .mockImplementationOnce(() => respond({ remaining: 2, verified: true }))
    .mockImplementationOnce(() => respond({ remaining: 1, message: 'Submitted' }))
    .mockImplementationOnce(() => respond({ remaining: 0, message: 'Submitted' }));
  render(<Demo />);
  expect(screen.getByRole('button', { name: 'Send test SMS' })).toBeDisabled();
  expect(screen.getByRole('button', { name: 'Send verification code' })).toBeDisabled();
  fireEvent.change(screen.getByLabelText(/Phone number/), { target: { value: '+12025550123' } });
  fireEvent.click(screen.getByRole('checkbox'));
  fireEvent.click(screen.getByRole('button', { name: 'Send verification code' }));
  fireEvent.change(await screen.findByLabelText(/Verification code/), { target: { value: '123456' } });
  fireEvent.click(screen.getByRole('button', { name: 'Verify phone' }));
  await waitFor(() => expect(screen.getByRole('button', { name: 'Send test SMS' })).toBeEnabled());
  fireEvent.click(screen.getByRole('button', { name: 'Send test SMS' }));
  expect(await screen.findByText('1 / 2 sends left')).toBeInTheDocument();
  fireEvent.click(screen.getByRole('button', { name: 'Send test SMS' }));
  expect(await screen.findByText('0 / 2 sends left')).toBeInTheDocument();
  expect(screen.getByRole('button', { name: 'Send test SMS' })).toBeDisabled();
  expect(JSON.parse(fetch.mock.calls[2][1].body)).toEqual({ template: 'recall' });
  expect(fetch.mock.calls[2][1].headers['X-Demo-Session']).toBe('session');
});

test('restores exhausted quota from server on refresh', async () => {
  sessionStorage.setItem('recallDemoSession', 'session');
  fetch.mockImplementationOnce(() => respond({ verified: true, remaining: 0, phone: '••• ••• 0123' }));
  render(<Demo />);
  expect(await screen.findByText('0 / 2 sends left')).toBeInTheDocument();
  expect(screen.getByRole('button', { name: 'Send test SMS' })).toBeDisabled();
});

test('failed send refreshes quota without retrying the SMS', async () => {
  sessionStorage.setItem('recallDemoSession', 'session');
  fetch.mockImplementationOnce(() => respond({ verified: true, remaining: 1, phone: '••• ••• 0123' }))
    .mockImplementationOnce(() => respond({ message: 'Provider unavailable' }, false, 502))
    .mockImplementationOnce(() => respond({ verified: true, remaining: 0 }));
  render(<Demo />);
  fireEvent.click(await screen.findByRole('button', { name: 'Send test SMS' }));
  expect(await screen.findByRole('alert')).toHaveTextContent('Provider unavailable');
  expect(screen.getByRole('button', { name: 'Send test SMS' })).toBeDisabled();
  expect(fetch.mock.calls.filter(([url]) => url.endsWith('/send'))).toHaveLength(1);
});

