import React, { useState, useEffect } from 'react';
import { useParams, useNavigate, useNavigationType } from 'react-router-dom';
import { Typography, Paper, Tabs, Tab, Box, Grid, Button } from '@mui/material';

import { DbButton } from './Buttons.js';
import { ToolBar } from './Miscelleneous.js';
import axios from 'axios';


const RecallStats = () => {
    const navigate = useNavigate();
    const { recallId } = useParams();
    const [recall, setRecall] = useState({
        timeStarted: '',
        timeEnded: '',
        message: ''
    });
    useEffect(() => {
        // Fetch contact data from the API
        axios.get('http://localhost:5000/api/recall/' + recallId) // Assuming endpoint to fetch contact details
            .then(response => {
                // Set the state with retrieved contact data
                setRecall(response.data)
                console.log(recall);
               
            })
            .catch(error => {
                console.error('Error fetching contact data:', error);
            });
    }, [recallId]);
    return (
        <div>
            <ToolBar />
            <Paper>
                <Typography variant="h4">Recall Details</Typography>
                <Grid container spacing={2}>
                    <Grid item xs={12}>
                        <Typography variant="h6">Message: {recall.message}</Typography>
                    </Grid>
                    <Grid item xs={12}>
                        <Typography variant="body1">Time Started: {recall.timeStarted}</Typography>
                    </Grid>
                    <Grid item xs={12}>
                        <Typography variant="body1">Time Ended: {recall.timeEnded}</Typography>
                    </Grid>
                    {/* Add more details if needed */}
                </Grid>
            </Paper>
            <Button variant="contained" onClick={() => navigate(-1)} fullWidth style={{ marginTop: '20px' }}>
                    Go Back
                </Button>
        </div>
    );
};



export default  RecallStats;