import { fireEvent, render, screen } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import axios from 'axios';
import { AuthProvider } from './Auth';
import Home from './nav/Home';
import LoginPage from './LoginPage';

jest.mock('axios', () => ({ post: jest.fn() }));
const renderPage = (page) => render(<AuthProvider><MemoryRouter>{page}</MemoryRouter></AuthProvider>);
beforeEach(() => { localStorage.clear(); jest.clearAllMocks(); });

test('home offers a login link and clear workflow', () => {
  renderPage(<Home />);
  expect(screen.getByRole('heading', { name: /stay connected/i })).toBeInTheDocument();
  expect(screen.getByRole('link', { name: /officer login/i })).toHaveAttribute('href', '/login');
  expect(screen.getAllByRole('article')).toHaveLength(3);
});

test('login reports rejected credentials', async () => {
  axios.post.mockRejectedValueOnce({ response: { status: 401 } });
  renderPage(<LoginPage />);
  expect(screen.getByLabelText(/email/i)).toBeRequired();
  fireEvent.change(screen.getByLabelText(/email/i), { target: { value: 'officer@example.com' } });
  fireEvent.change(screen.getByLabelText(/password/i), { target: { value: 'invalid-password' } });
  fireEvent.click(screen.getByRole('button', { name: 'Sign in' }));
  expect(await screen.findByRole('alert')).toHaveTextContent('The email or password is incorrect');
  expect(screen.getByRole('button', { name: 'Sign in' })).toBeEnabled();
});

test('login prevents duplicate requests and reports connectivity failures', async () => {
  let rejectRequest;
  axios.post.mockImplementationOnce(() => new Promise((_, reject) => { rejectRequest = reject; }));
  renderPage(<LoginPage />);
  fireEvent.change(screen.getByLabelText(/email/i), { target: { value: 'officer@example.com' } });
  fireEvent.change(screen.getByLabelText(/password/i), { target: { value: 'test-password' } });
  fireEvent.click(screen.getByRole('button', { name: 'Sign in' }));
  expect(screen.getByRole('button', { name: /signing in/i })).toBeDisabled();
  fireEvent.submit(screen.getByLabelText(/email/i).closest('form'));
  expect(axios.post).toHaveBeenCalledTimes(1);
  rejectRequest(new Error('Network unavailable'));
  expect(await screen.findByRole('alert')).toHaveTextContent('Unable to sign in right now');
});
