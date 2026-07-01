import { useState, useEffect } from 'react';
import axios from 'axios';

const useRoster = () => {
  const [rosters, setRosters] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchRosters = async () => {
      try {
        setLoading(true);
        const response = await axios.get('http://localhost:5000/api/roster');
        setRosters(response.data);
        setLoading(false);
      } catch (error) {
        setError(error);
        setLoading(false);
      }
    };

    fetchRosters();
  }, []);

  return { rosters, loading, error };
};

export default useRoster;