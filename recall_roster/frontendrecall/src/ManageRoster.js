import React from 'react';
import { Button } from '@mui/material';
import { Link } from 'react-router-dom';
import useRoster from './UseRoster'; // Adjust the path as needed
import { NavyButton } from './Buttons';
import { useNavigate } from 'react-router-dom';
import { ToolBar } from './Miscelleneous';
import './css/ItemRows.css'


export const RemoveContact = ({ children }) => (
  <span className="button">{children}</span>
);

const ManageRoster = () => {
  const { rosters, loading, error } = useRoster();
const navigate = useNavigate();
  if (loading) {
    return <div>Loading...</div>;
  }

  if (error) {
    return <div>Error: {error.message}</div>;
  }

  const handleRemove = async (id) => {
    try {
      const response = await fetch(`http://localhost:5000/api/roster/remove/${id}`, {
        method: 'DELETE',
      });
      if (!response.ok) {
        throw new Error('Network response was not ok');
      }
      console.log('Contact removed:', await response.json());
    } catch (error) {
      console.error('There was a problem removing the roster:', error);
    }
  };

  const handleEdit = (id) => {
    navigate("/editRoster/" +id)
  }
  return (
    <div>
    
      <div className="contact-list-container">
        <div className="contact-list-section">
          <h1>Roster List</h1>
          <hr />
          <ul className="contact-list">
            {rosters.map((roster) => (
              <li key={roster.rosterId} style={{ color: 'black' }}>
                <h2 className="list" style={{ color: 'black' }}>{roster.name}</h2>
             
                  
                    <NavyButton size="large" variant="contained" color="primary" onClick={() => handleEdit(roster.rosterId)}>Edit</NavyButton>
                  
                  <Button
                    size="large"
                    variant="contained"
                    style={{ backgroundColor: 'red', color: 'white' }}
                    onClick={() => handleRemove(roster.rosterId)}
                  >
                    Remove
                  </Button>
                  <hr></hr>
              </li>
            ))}
            
          </ul>
    
        </div>
      </div>
    </div>
  );
};

export default ManageRoster;