import React, { useState, useEffect } from 'react';
import { Typography, Button, Container, TextField, Select, MenuItem, Snackbar, Alert } from '@mui/material';
import { ToolBar } from './Miscelleneous.js';
import './css/Landing.css';
import { useNavigate } from 'react-router-dom';
const InsertContact = () => {
    const [firstName, setFirst] = useState('');
    const [lastName, setLast] = useState('');
    const [phoneNumber, setPhoneNumber] = useState('');
    const [selectedOption, setSelectedOption] = useState('');
    const [alertOpen, setAlertOpen] = useState(false);
    const navigate = useNavigate();
    const handleCloseAlert = () => {
        setAlertOpen(false);
    };

    const data = {
        firstName,
        lastName,
        phoneNumber,
        active: 1,
        Rank: selectedOption
    };

    const handleAdd = async () => {
        console.log(data);
        fetch('http://localhost:5000/api/Contact', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
        })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            // Handle success
            console.log('Contact added:', data);
            // Clear textboxes
            setFirst('');
            setLast('');
            setPhoneNumber('');
            setSelectedOption('');
            // Display alert
            setAlertOpen(true);
        })
        .catch(error => {
            // Handle error
            console.error('There was a problem adding the contact:', error);
        });
    };

    
    // useEffect(() => {
    //     setAlertOpen(true); // Display the alert when the component mounts
    // }, []);

    return (
        <div className="h">
            <ToolBar />
             {/* Display a Snackbar for the alert */}
             <Snackbar open={alertOpen} autoHideDuration={6000} onClose={handleCloseAlert} anchorOrigin={{
        vertical: 'top',
        horizontal: 'right',
        
    }}
    
    sx={{
      // Adjust width as needed
        minHeight: '100px', // Adjust height as needed
    }}>
                    <Alert onClose={handleCloseAlert} severity="success" sx={{ width: '100%', fontSize: '1.2rem' }}>
                        Contact added successfully!
                    </Alert>
                </Snackbar>
            <Container style={{ padding: '25px', borderRadius: 16 }}>
                <Typography variant="h4" align="center" gutterBottom className="logoName">
                    Please enter your Contact Details
                </Typography>
                <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                    <span style={{ marginLeft: '13px' }}>
                        <Typography className="login" variant="h4" align="center" gutterBottom margin='10px' padding='20px'>
                            First Name    
                        </Typography>
                    </span>
                    <span style={{ marginLeft: '10px' }}>
                        <TextField label="First name" variant="outlined" value={firstName} onChange={(e => setFirst(e.target.value))} sx={{ textAlign: 'center' }} InputProps={{ style: { borderColor: 'white' } }} />
                    </span>
                </div>
                <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', marginTop: '20px' }}>
                    <Typography className="login" variant="h4" align="center" gutterBottom margin='12px' padding-right='30px'>
                        Last Name
                    </Typography>
                    <TextField label="Last name" variant="outlined" value={lastName} onChange={(e => setLast(e.target.value))} sx={{ textAlign: 'center' }} margin='20px' />
                </div>
                <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', marginTop: '20px' }}>
                    <Typography className="login" variant="h4" align="center" gutterBottom margin='12px' padding-right='30px'>
                        Phone Number
                    </Typography>
                    <TextField label="123-456-7899" variant="outlined" value={phoneNumber} onChange={(e => setPhoneNumber(e.target.value))} sx={{ textAlign: 'center' }} margin='20px' />
                </div>
                <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', marginTop: '20px' }}>
                    <Typography className="login" variant="h4" align="center" gutterBottom margin='12px' padding-right='30px'>Rank</Typography>
                    <Select 
                        value={selectedOption}
                        onChange={(e) => setSelectedOption(e.target.value)}
                        variant="outlined"
                        sx={{ minWidth: 200 }}
                    >
                        <MenuItem value="">Select Option</MenuItem>
                        <MenuItem value="Employee">Employee</MenuItem>
                        <MenuItem value="Element Chief">Element Chief</MenuItem>
                        <MenuItem value="Flight Chief">Flight Chief</MenuItem>
                        <MenuItem value="Squadron Director">Squadron Director</MenuItem>
                    </Select>
                </div>
                <div style={{ marginTop: '20px', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                    <Button size="large" variant="contained" color="primary" onClick={() => handleAdd()}>
                        Submit
                    </Button>
                    <Button variant="contained" onClick={() => navigate(-1)} style={{ position: 'fixed', bottom: '20px', left: '20px' }}>Go Back</Button>
                </div>
               
            </Container>
        </div>
    );
};

export default InsertContact;
