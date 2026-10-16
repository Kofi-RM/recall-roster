import { useState, useEffect } from 'react';
import api from '../api/api';

const useRosterContacts = (id) => {
  const [rosterContacts, setRosterContacts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchRosterContacts = async () => {
      try {
        setLoading(true);
        const response = await api.get('/rostercontact/' + id);
        setRosterContacts(response.data);
        setLoading(false);
      } catch (error) {
        setError(error);
        setLoading(false);
      }
    };

    fetchRosterContacts();
  }, [id]);

  return { rosterContacts, loading, error };
};

export default useRosterContacts;