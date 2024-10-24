import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { Typography, Paper, Tabs, Tab, Grid, Button, LinearProgress } from '@mui/material';
import axios from 'axios';

const RecallStats = () => {
    const navigate = useNavigate();
    const { recallId } = useParams();
    const [recall, setRecall] = useState({
        timeStarted: '',
        timeEnded: '',
        message: ''
    });
    const [contacts, setContacts] = useState([]);
    const [activeTab, setActiveTab] = useState('all');
    let rosterId;

    useEffect(() => {
        console.log(recallId);
        // Fetch recall details
        axios.get('http://localhost:5000/api/recall/' + recallId)
        .then(response => {
            setRecall(response.data)
            console.log( response.data);
            const rosterId = response.data.rosterId;
            console.log("rosterId" + rosterId);
    
            // Fetch contacts associated with the recall
            axios.get('http://localhost:5000/api/rostercontact/' + rosterId)
                .then(rosterContactResponse => {
                    // Iterate through rosterContacts and make individual calls for each contact
                    console.log(rosterContactResponse);
                    Promise.all(rosterContactResponse.data.map(rc => axios.get(`http://localhost:5000/api/contact/${rc.contactId}`)))
                        .then(contactResponses => {
                            // Process each contact response
                            const contactsData = contactResponses.map(contactResponse => contactResponse.data);
                            console.log(contactsData);
                            setContacts(contactsData);
                        })
                        .catch(error => {
                            console.error('Error fetching contact data:', error);
                        });
                })
                .catch(error => {
                    console.error('Error fetching roster contact data:', error);
                });
        })
        .catch(error => {
            console.error('Error fetching recall data:', error);
        });
    }, [recallId]);

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
    filteredContacts.map(contact => (
        <div key={contact.contactId} style={{ backgroundColor: 'white', padding: '10px', margin: '10px', borderRadius: '5px' }}>
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
