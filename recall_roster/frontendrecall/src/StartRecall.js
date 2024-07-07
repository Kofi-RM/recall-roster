
import React, { useState, useEffect } from 'react';
import { Typography, Button, Container, TextField, Select, MenuItem, Snackbar, Alert } from '@mui/material';
import { ToolBar } from './Miscelleneous.js';
import './css/Landing.css';
import { useNavigate } from 'react-router-dom';
import useRoster from './UseRoster.js';
import axios from 'axios';
import { NavyButton } from './Buttons.js';

const StartRecall = () => {
    const [selectedRoster, setSelectedRoster] = useState('');
    const [message, setMessage] = useState('');
    const { rosters, loading, error } = useRoster();
    const [isSubmitting, setIsSubmitting] = useState(false);
 
    const [alertOpen, setAlertOpen] = useState(false);
    const navigate = useNavigate();
  
    const handleCloseAlert = () => {
        setAlertOpen(false);
    };


     const addRecall  = async () => {
        let employeesMax = 0;
        let flightChiefMax = 0;
        let elementChiefMax = 0;
        let squadronDirectorMax = 0;
        const data = {
            rosterId: selectedRoster,
            message: message,
            timeStarted: "2024-04-04T12:00:00",
            timeEnded: "2024-04-04T12:00:00",
            active: 1,
            Employees: 0,
            FlightChief: 0,
            ElementChief: 0,
            SquadronDirector: 0,
            
            EmployeesMax: 0,
            FlightChiefMax: 0,
            ElementChiefMax: 0,
            SquadronDirectorMax: 0,
            TotalMax: 0


        };
        
        
        const { rosterContacts } = await fetchRosterContacts(selectedRoster);

        for (const rc of rosterContacts)
            {
                const id = rc.contactId;
                const response = await axios.get(`http://localhost:5000/api/contact/${id}`);
                const role = response.data.role;
                console.log(role + "role")
                switch (role) {
                case 'Employee':
                    employeesMax++;
                    break;
                case 'Flight Chief':
                    flightChiefMax++;
                    break;
                case 'Element Chief':
                    elementChiefMax++;
                    break;
                case 'Squadron Director':
                    squadronDirectorMax++;
                    break;
                default:
                    break;
            }
     };
    
     data.EmployeesMax = employeesMax;
     console.log(employeesMax);
data.FlightChiefMax = flightChiefMax;
data.ElementChiefMax = elementChiefMax;
data.SquadronDirectorMax = squadronDirectorMax;
data.TotalMax = employeesMax + flightChiefMax + elementChiefMax + squadronDirectorMax;

        data.TotalMax = data.EmployeesMax + data.FlightChiefMax + data.ElementChiefMax + data.SquadronDirectorMax;
        console.log(data);
      

        axios.post('http://localhost:5000/api/Recall', data)
        .then(response => {
            console.log('Recall data posted successfully:', response.data);
            // Handle response as needed
        })
        .catch(error => {
            console.error('Error posting recall data:', error);
            // Handle error as needed
        });


    }

    const fetchRosterContacts = async (selectedRoster) => {
        try {
            const response = await axios.get(`http://localhost:5000/api/rostercontact/${selectedRoster}`);
            const rosterContacts = response.data;
            return { rosterContacts };
        } catch (error) {
            console.error('Error fetching roster contacts:', error);
            return { rosterContacts: [] };
        }
    };
    const handleSubmit = () => {
        // // Logic to submit the message
       
        setIsSubmitting(true);
        addRecall();
    };

    return (
        <div className="background">
            <ToolBar />
            {/* Display a Snackbar for the alert */}
            <Snackbar open={alertOpen} autoHideDuration={6000} onClose={handleCloseAlert} anchorOrigin={{
                vertical: 'top',
                horizontal: 'right',
            }}>
                <Alert onClose={handleCloseAlert} severity="success" sx={{ width: '100%', fontSize: '1.2rem' }}>
                    Message sent successfully!
                </Alert>
            </Snackbar>
            <Container style={{ padding: '25px', borderRadius: 16 }}>
                <Typography variant="h4" align="center" gutterBottom className="logoName">
                   Initiate Recall
                </Typography>
                <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', marginTop: '20px' }}>
                    <Typography className="login" variant="h4" align="center" gutterBottom margin='12px' padding-right='30px'>Roster</Typography>
                    {rosters.length > 0 && (
  <Select 
    value={selectedRoster}
    onChange={(e) => setSelectedRoster(e.target.value)}
    variant="outlined"
    sx={{ minWidth: 200 }}
  >
    <MenuItem value="">Select Roster</MenuItem>
    {rosters.map(roster => (
      <MenuItem key={roster.rosterId} value={roster.rosterId}>{roster.name}</MenuItem>
    ))}
  </Select>
)}
                </div>
                <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', marginTop: '20px' }}>
                    <Typography className="login" variant="h4" align="center" gutterBottom margin='12px' padding-right='30px'>Message</Typography>
                    <TextField 
                        label="Type your message here"
                        variant="outlined"
                        multiline
                        rows={6}
                        value={message}
                        onChange={(e) => setMessage(e.target.value)}
                        sx={{ minWidth: 400 }}
                    />
                </div>
                <div style={{ marginTop: '20px', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                    <NavyButton size="large" variant="contained" color="primary" onClick={handleSubmit}>
                        Start Recall
                    </NavyButton>
                    <NavyButton variant="contained" onClick={() => navigate(-1)} style={{ position: 'sticky', bottom: '20px', left: '20px' }}>Go Back</NavyButton>
                </div>
            </Container>
        </div>
    );
};

export default StartRecall;
