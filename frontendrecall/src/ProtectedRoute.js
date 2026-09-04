import { Navigate, useLocation } from 'react-router-dom';
import { useAuth } from './Auth';

export default function ProtectedRoute({ children }) {
  const { isLoggedIn } = useAuth();
  const location = useLocation();
  return isLoggedIn ? children : <Navigate to="/login" replace state={{ from: location.pathname + location.search + location.hash }} />;
}
