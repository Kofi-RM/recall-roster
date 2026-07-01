import { useState, useEffect } from 'react';
import axios from 'axios';

const useRosterContacts = (id) => {
  const [rosterContacts, setRosterContacts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchRosterContacts = async () => {
      try {
        setLoading(true);
        const response = await axios.get('http://localhost:5000/api/rostercontact' + id);
        setRosterContacts(response.data);
        setLoading(false);
      } catch (error) {
        setError(error);
        setLoading(false);
      }
    };

    fetchRosterContacts();
  }, []);

  return { rosterContacts, loading, error };
};

export default useRosterContacts;