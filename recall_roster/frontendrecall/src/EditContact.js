import React, { useState, useEffect } from 'react';
import { Typography, Button, Container, TextField } from '@mui/material';
import { useParams } from 'react-router-dom';
import axios from 'axios';
import {ToolBar, MyImage} from './Miscelleneous.js'

const EditContact = () => {
    const { contactId } = useParams();
    const [contact, setContact] = useState({
        firstName: '',
        lastName: '',
        phoneNumber: ''
    });

    useEffect(() => {
        // Fetch contact data from the API
        axios.get('http://localhost:5000/api/contact/' + contactId) // Assuming endpoint to fetch contact details
            .then(response => {
                // Set the state with retrieved contact data
                setContact(response.data);
            })
            .catch(error => {
                console.error('Error fetching contact data:', error);
            });
    }, []);

    const handleChange = (e) => {
        const { name, value } = e.target;
        setContact(prevState => ({
            ...prevState,
            [name]: value
        }));
    };

    const handleSubmit = () => {
        // Logic to submit updated contact data
    };

    return (
        <div>
        <ToolBar></ToolBar>
        <Container style={{ display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
            <Typography variant="h4" align="center" gutterBottom color = "white">
                Alter this Contact
            </Typography>
            <div>
            <Typography variant="h4" align="center" gutterBottom color = "white">
                First Name
            </Typography>
            <TextField
               // label="First Name"
                variant="outlined"
                name="firstName"
                value={contact.firstName}
                onChange={handleChange}
                sx={{ backgroundColor: '#e0e0e0', margin: '15px' }}
            />
            </div>
            <div>
            <Typography variant="h4" align="center" gutterBottom color = "white">
               Last Name
            </Typography>
            <TextField
               // label="Last Name"
                variant="outlined"
                name="lastName"
                value={contact.lastName}
                onChange={handleChange}
                sx={{ backgroundColor: '#e0e0e0', margin: '15px' }}
            />
            </div>
            <div>
            <Typography variant="h4" align="center" gutterBottom color = "white">
               Phone Number
            </Typography>
            <TextField
               // label="Phone Number"
                variant="outlined"
                name="phoneNumber"
                value={contact.phoneNumber}
                onChange={handleChange}
                sx={{ backgroundColor: '#e0e0e0', margin: '15px' }}
            />
            </div>
            <Button variant="contained" color="primary" onClick={handleSubmit}>
                Update Contact
            </Button>
        </Container>
        </div>
    );
};

export default EditContact;
