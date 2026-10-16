import { useState, useEffect } from 'react';
import api from '../api/api';

const useActiveRecalls = () => {
  const [recallsActive, setRecalls] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  useEffect(() => {
    let cancelled = false;
    let timer;
    const refresh = async () => {
      try {
        const response = await api.get('/recall');
        if (!cancelled) { setRecalls(response.data); setError(null); }
      } catch (failure) {
        if (!cancelled) { setRecalls([]); setError(failure); }
      } finally {
        if (!cancelled) { setLoading(false); timer = setTimeout(refresh, 5000); }
      }
    };
    refresh();
    return () => { cancelled = true; clearTimeout(timer); };
  }, []);
  return [recallsActive, loading, error];
};
export default useActiveRecalls;
