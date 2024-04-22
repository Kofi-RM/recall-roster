import React, { useState, useEffect } from 'react';
import { Typography, Button, Container, TextField } from '@mui/material';
import { useParams, useNavigate } from 'react-router-dom';
import axios from 'axios';
import {ToolBar, MyImage} from './Miscelleneous.js'

const EditContact = () => {
    const navigate = useNavigate();
    const { contactId } = useParams();
    const [contact, setContact] = useState({
        firstName: '',
        lastName: '',
        phoneNumber: ''
    });

    const [initialContact, setInitialContact] = useState(null);
    useEffect(() => {
        // Fetch contact data from the API
        axios.get('http://localhost:5000/api/contact/' + contactId) // Assuming endpoint to fetch contact details
            .then(response => {
                // Set the state with retrieved contact data
                setContact(response.data);
                setInitialContact(response.data);
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
        axios.put(`http://localhost:5000/api/contact/${contactId}`, contact)
        .then(response => {
            console.log('Contact updated successfully:', response.data);
            navigate("/manageContacts");
            console.log("navigate");
            
            // Optionally, you can perform further actions after the contact has been updated, such as redirecting to another page or displaying a success message.
        })
        .catch(error => {
            console.error('Error updating contact:', error);
            // Optionally, you can handle errors here, such as displaying an error message to the user.
        });
    };

    return (
        <div>
        <ToolBar></ToolBar>
        <Container style={{ display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
            <Typography variant="h4" align="center" gutterBottom color = "white">
                Alter this Contact
            </Typography>
            <div>
            {initialContact && (
        <div style={{ textAlign: 'center' }}>
            <Typography variant="h4" align="center" gutterBottom color="white">
                First Name: {initialContact.firstName}
            </Typography>
            <TextField
                variant="outlined"
                name="firstName"
                value={contact.firstName}
                onChange={handleChange}
                sx={{ backgroundColor: '#e0e0e0', margin: '15px' }}
            />
        </div>
    )}
    {initialContact && (
        <div style={{ textAlign: 'center' }}>
            <Typography variant="h4" align="center" gutterBottom color="white">
                Last Name: {initialContact.lastName}
            </Typography>
            <TextField
                variant="outlined"
                name="lastName"
                value={contact.lastName}
                onChange={handleChange}
                sx={{ backgroundColor: '#e0e0e0', margin: '15px' }}
            />
        </div>
    )}
    {initialContact && (
        <div style={{ textAlign: 'center' }}>
            <Typography variant="h4" align="center" gutterBottom color="white">
                Phone Number: {initialContact.phoneNumber}
            </Typography>
            <TextField
                variant="outlined"
                name="phoneNumber"
                value={contact.phoneNumber}
                onChange={handleChange}
                sx={{ backgroundColor: '#e0e0e0', margin: '15px' }}
            />
        </div>
    )}
            </div>
            <Button variant="contained" color="primary" onClick={handleSubmit}>
                Update Contact
            </Button>
            <Button variant="contained" color="primary" onClick={() => window.history.back()}>Go Back</Button>
        </Container>
        </div>
    );
};

export default EditContact;
