import { useState, useEffect } from 'react';
import axios from 'axios';

const useActiveRecalls = () => {
  const [recallsActive, setRecalls] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchRecalls = async () => {
      try {
        setLoading(true);
        const response = await axios.get('http://localhost:5000/api/recall');
        setRecalls(response.data);
        setLoading(false);
      } catch (error) {
        setError(error);
        setLoading(false);
      }
    };

    fetchRecalls();
  }, []);

  return [ recallsActive, loading, error ];
};

export default useActiveRecalls;