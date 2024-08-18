import React, { useEffect, useState } from 'react';
import { Typography, Button, Container, TextField, Snackbar, Alert } from '@mui/material';
import { ToolBar } from './Miscelleneous.js';
import { useNavigate } from 'react-router-dom';
import useContacts from './UseContacts.js';
import axios from 'axios';

const CreateRoster = () => {
    const [rosterName, setRosterName] = useState('');
    const [description, setDescription] = useState('');
    const [alertOpen, setAlertOpen] = useState(false);
    const { contacts, loading, error } = useContacts();
    const [contactsByRole, setContactsByRole] = useState({});
    const [selectedContacts, setSelectedContacts] = useState([]);
    const [rosterContacts, setRosterContacts] = useState([]);
    const navigate = useNavigate();


    useEffect(() => {
        if (contacts && contacts.length > 0) {
        const groupedContacts = groupContactsByRole(contacts);
        setContactsByRole(groupedContacts);
        }
    }) 
    const handleCloseAlert = () => {
        setAlertOpen(false);
    };

    const handleAddRoster = async () => {
        // Create the roster with name and description
        const rosterData = {
            name: rosterName,
            description: description
        };
    
        try {
            // Send a POST request to create the roster
            const rosterResponse = await axios.post('http://localhost:5000/api/roster', rosterData);
    
            const rosterId = rosterResponse.data.rosterId;
    
            // Create roster contact entries
            const rosterContactData = {
                rosterId: rosterId,
                contactIds: rosterContacts
            };
    
           const  contactsToRemove = [];
            // Send a POST request to create roster contacts
            axios.post('http://localhost:5000/api/RosterContact/updateContacts', {
            rosterId: rosterId,
            contactsToAdd: rosterContactData,
            contactsToRemove: contactsToRemove
           
        }) .then(response => {
            console.log('Roster contacts updated successfully:', response.data);
            // Optionally, you can update the local state or perform any other actions upon successful update.
        })
        .catch(error => {
            console.error('Error updating roster contacts:', error);
            // Optionally, you can handle errors here, such as displaying an error message to the user.
        });
    
            // Display success message
           // console.log('Roster and roster contacts created successfully');
            setAlertOpen(true);
            // Reset form fields
            setRosterName('');
            setDescription('');
            setRosterContacts([]);
    
            // Redirect to the previous page
        } catch (error) {
            console.error('Error:', error.message);
            // Handle error (e.g., display error message)
        }
    };


    const handleCheckboxChange = (contactID) => {
        // Check if the contactID is already in rosterContacts
        const contactIndex = rosterContacts.indexOf(contactID);
        if (contactIndex === -1) {
            // If not found, add it to the array
            setRosterContacts(prevRosterContacts => [...prevRosterContacts, contactID]);
        } else {
            // If found, remove it from the array
            setRosterContacts(prevRosterContacts => {
                const updatedRosterContacts = [...prevRosterContacts];
                updatedRosterContacts.splice(contactIndex, 1);
                return updatedRosterContacts;
            });
        }
    };
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

   

    return (
        <div>
            <ToolBar />
            {/* Display a Snackbar for the alert */}
            <Snackbar
                open={alertOpen}
                autoHideDuration={6000}
                onClose={handleCloseAlert}
                anchorOrigin={{
                    vertical: 'top',
                    horizontal: 'right',
                }}
            >
                <Alert onClose={handleCloseAlert} severity="success" sx={{ width: '100%', fontSize: '1.2rem' }}>
                    Roster added successfully!
                </Alert>
            </Snackbar>
            <Container style={{ padding: '25px', borderRadius: 16 }}>
    <Typography variant="h4" align="center" gutterBottom className="logoName">
        Please enter the Roster Details
    </Typography>
    <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
        <TextField
            label="Roster Name"
            variant="outlined"
            value={rosterName}
            onChange={e => setRosterName(e.target.value)}
            sx={{ textAlign: 'center' }}
            InputProps={{ style: { borderColor: 'white' } }}
        />
    </div>
    <div style={{ marginTop: '20px', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
        <TextField
            label="Description"
            variant="outlined"
            multiline
            rows={4}
            value={description}
            onChange={e => setDescription(e.target.value)}
            sx={{ textAlign: 'center' }}
            InputProps={{ style: { borderColor: 'white' } }}
        />
    </div>

    {/* Right Panel for Listing Contacts */}
    <div style={{ marginTop: '20px', padding: '20px', border: '1px solid #ccc', borderRadius: '8px' }}>
        <Typography variant="h5" align="center" gutterBottom color="primary">
            Contacts
        </Typography>
        <div style={{ display: 'flex', flexDirection: 'row', justifyContent: 'space-between' }}>
            {Object.entries(contactsByRole).map(([role, contacts]) => (
                <div key={role} style={{ marginTop: '20px', flexBasis: '30%' }}>
                    <Typography variant="h6" gutterBottom color="primary">
                        {role}
                    </Typography>
                    <ul>
                        {contacts.map(contact => (
                            <li key={contact.contactID} style={{ display: 'flex', alignItems: 'center', marginBottom: '5px' }}>
                                <Typography variant="body1" style={{ marginRight: '10px' }}>
                                    {contact.firstName} {contact.lastName}
                                </Typography>
                                <input
                                    type="checkbox"
                                    value={contact.contactID}
                                    onChange={() => handleCheckboxChange(contact.contactID)}
                                    checked={rosterContacts.includes(contact.contactID)}
                                />
                            </li>
                        ))}
                    </ul>
                </div>
            ))}
        </div>
    </div>

    <div style={{ marginTop: '20px', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
        <Button size="large" variant="contained" color="primary" onClick={handleAddRoster}>
            Submit
        </Button>
        <Button variant="contained" onClick={() => navigate(-1)} style={{ marginLeft: '20px' }}>
            Go Back
        </Button>
    </div>
</Container>
        </div>
    );
};

export default CreateRoster;
