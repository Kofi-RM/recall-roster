import React, { useState, useEffect } from 'react';
import { Typography, Button, Container, TextField } from '@mui/material';
import { useParams, useNavigate } from 'react-router-dom';
import axios from 'axios';
import { ToolBar } from './Miscelleneous.js';
import useContacts from './UseContacts.js'; // Adjust the path as needed

const EditRoster = () => {
    const navigate = useNavigate();
    const { rosterId } = useParams();
    const [roster, setRoster] = useState({
        name: '',
        description: '',
    });
    const [initialRoster, setInitialRoster] = useState(null);
    const { contacts, loading, error } = useContacts();
    const [contactsByRole, setContactsByRole] = useState({});

    useEffect(() => {
        // Function to group contacts by role
        const groupContactsByRole = (contacts) => {
            const groupedContacts = {};
            contacts.forEach(contact => {
                if (!groupedContacts[contact.role]) {
                    groupedContacts[contact.role] = [];
                }
                groupedContacts[contact.role].push(contact);
            });
            // Sort contacts alphabetically within each role
            for (const role in groupedContacts) {
                groupedContacts[role].sort((a, b) => a.lastName.localeCompare(b.lastName));
            }
            return groupedContacts;
        };

        if (contacts.length > 0) {
            const groupedContacts = groupContactsByRole(contacts);
            setContactsByRole(groupedContacts);
        }
    }, [contacts]);

    useEffect(() => {
        // Fetch contact data from the API
        axios.get('http://localhost:5000/api/roster/' + rosterId)
            .then(response => {
                // Set the state with retrieved contact data
                setRoster(response.data);
                setInitialRoster(response.data);
                // Group contacts by role and update state
            
            })
            .catch(error => {
                console.error('Error fetching contact data:', error);
            });
    }, [rosterId, contacts]); // Add dependencies to prevent unnecessary re-renders

    const handleChange = (e) => {
        const { name, value } = e.target;
        setRoster(prevState => ({
            ...prevState,
            [name]: value
        }));
    };

    const handleSubmit = () => {
        // Logic to submit updated contact data
        axios.put(`http://localhost:5000/api/roster/update/${rosterId}`, roster)
            .then(response => {
                console.log('Contact updated successfully:', response.data);
                window.location.reload();
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
            <ToolBar />
            <Container style={{ display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
                <Typography variant="h4" align="center" gutterBottom color="white">
                    Alter this Contact
                </Typography>
                {initialRoster && (
                    <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
                        <Typography variant="h4" align="center" gutterBottom color="white">
                            Roster Name: {initialRoster.name}
                        </Typography>
                        <TextField
                            variant="outlined"
                            name="name"
                            value={roster.name}
                            onChange={handleChange}
                            sx={{ backgroundColor: '#e0e0e0', margin: '15px' }}
                        />
                    </div>
                )}
                {initialRoster && (
                    <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
                        <Typography variant="h4" align="center" gutterBottom color="white">
                            Description: {initialRoster.description}
                        </Typography>
                        <TextField
                            variant="outlined"
                            name="description"
                            value={roster.description}
                            onChange={handleChange}
                            sx={{ backgroundColor: '#e0e0e0', margin: '15px' }}
                        />
                    </div>
                )}

                <Button variant="contained" color="primary" onClick={handleSubmit}>
                    Update Roster
                </Button>
                <button onClick={() => window.history.back()}>Go Back</button>

                {/* Display contacts grouped by role */}
                {Object.entries(contactsByRole).map(([role, contacts]) => (
                    <div key={role}>
                        <Typography variant="h5" gutterBottom>{role}</Typography>
                        <ul>
                            {contacts.map(contact => (
                                <li key={contact.id}>
                                    {contact.firstName} {contact.lastName}
                                </li>
                            ))}
                        </ul>
                    </div>
                ))}
            </Container>
        </div>
    );
};

export default EditRoster;
