import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { Typography, Paper, Tabs, Tab, Grid, Button, LinearProgress } from '@mui/material';
import axios from 'axios';
import api from '../api/api';

const RecallStats = () => {
    const navigate = useNavigate();
    const { recallId } = useParams(); // snag recallId from URL params
    const [recall, setRecall] = useState({
        timeStarted: '',
        timeEnded: '',
        message: ''
    });
    const [responses, setResponses] = useState({});
    
    const [contacts, setContacts] = useState([]);
    const [activeTab, setActiveTab] = useState('all');
    let rosterId;

    useEffect(() => {
        console.log(recallId);
        // Fetch recall details
        api.get('http://localhost:5000/api/recall/' + recallId)
        .then(response => {
            setRecall(response.data)
            console.log( response.data);
            const rosterId = response.data.rosterId;
            console.log("rosterId" + rosterId);
            console.log("recallId" + recallId);
            // Fetch contacts associated with the recall
            api.get('http://localhost:5000/api/rostercontact/' + rosterId)
                .then(rc => {
                   return Promise.all(rc.data.map(rc => api.get(`http://localhost:5000/api/contact/${rc.contactId}`)))
                    .then(contactResponses => {
                        const contactsData = contactResponses.map(response => response.data);
                        setContacts(contactsData);
                        // maps each response to an array of contacts
                    })
                })
                .catch(error => {
                    console.error('Error fetching roster contact data:', error);
                });
        })
        .catch(error => {
            console.error('Error fetching recall data:', error);
        });
        
    }, [recallId]);

  useEffect(() => {
    const fetchResponses = async () => {
      try {
        const updatedContacts = await Promise.all(
          contacts.map(async contact => {
            if (contact.responded !== undefined) return contact; // Already has response info
            try {
              await api.get(`http://localhost:5000/api/Response/${recallId}/${contact.contactId}`);
              return { ...contact, responded: true };
            } catch (error) {
              console.error(`Error fetching response for ${contact.contactId}:`, error.message);
              return { ...contact, responded: false };
            }
          })
        );
        setContacts(updatedContacts);
      } catch (error) {
        console.error("Error fetching responses:", error.message);
      }
    };

    if (contacts.length > 0) {
      fetchResponses();
    }
  }, [recallId, contacts.length]);
    // Filter contacts based on role
    const filteredContacts = contacts.filter(contact => {
        if (activeTab === 'all') return true;
        return contact.rank === activeTab;
    });

    const calculateProgress = (rank) => {
        if (rank === 'all') {
            const totalContacts = contacts.length;
            const respondedContacts = contacts.filter(contact => contact.responded).length;
            if (totalContacts === 0) return 0; // to prevent division by zero
            return (respondedContacts / totalContacts) * 100;
        } else {
            const totalContacts = contacts.filter(contact => contact.rank === rank).length;
            const respondedContacts = filteredContacts.filter(contact => contact.rank === rank && contact.responded).length;
            if (totalContacts === 0) return 0; // to prevent division by zero
            return (respondedContacts / totalContacts) * 100;
        }
    };

    return (
        <div style={{ backgroundColor: 'white', padding: '20px', borderRadius: '5px' }}>
            <Paper>
                <Typography variant="h4">Recall Details</Typography>
                {/* Display recall details */}
            </Paper>

            {/* Tabs for different roles */}
            <Tabs value={activeTab} onChange={(event, newValue) => setActiveTab(newValue)} aria-label="roles">
                <Tab label="All" value="all" />
                <Tab label="Employees" value="Employee" />
                <Tab label="Element Chiefs" value="Element Chief" />
                <Tab label="Flight Chiefs" value="Flight Chief" />
                <Tab label="Squadron Directors" value="Squadron Director" />
            </Tabs>

            <Typography variant="body1">
    Current Progress: {calculateProgress(activeTab)}%
</Typography>
            <LinearProgress 
    variant="determinate" 
    value={calculateProgress(activeTab)} 
    style={{ margin: '20px 0', height: '10px' }} // Adjusted margin and height
/>
            {filteredContacts.length === 0 ? (
    <Typography variant="body1">You have no contacts of this rank</Typography>
) : (
    filteredContacts.map((contact, index) => (
        <div  key={`${contact.contactId}-${index}`} style={{ backgroundColor: 'white', padding: '10px', margin: '10px', borderRadius: '5px' }}>
            <Grid container spacing={2}>
                <Grid item xs={6}>
                    <Typography variant="body1">Name:{contact.firstName + " " + contact.lastName}</Typography>
                    <Typography variant="body2">Rank: {contact.rank}</Typography>
                </Grid>
                <Grid item xs={6}>
                    {/* Show response status */}
                    <Typography variant="body1">{contact.responded ? 'Responded' : 'Not Responded'}</Typography>
                </Grid>
                <Grid item xs={12}>
                    {/* Progress bar */}
                   
                </Grid>
            </Grid>
        </div>
    ))
)}

            <Button variant="contained" onClick={() => navigate(-1)} fullWidth style={{ marginTop: '20px' }}>
                Go Back
            </Button>
        </div>
    );
};

export default RecallStats;
