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
    const [selectedContacts, setSelectedContacts] = useState([]);
    const [rosterContacts, setRosterContacts] = useState([]);

    useEffect(() => {
        //console.log('EditRoster component mounted');

        // Fetch roster data from the API
        axios.get('http://localhost:5000/api/roster/' + rosterId)
            .then(response => {
                // Set the state with retrieved roster data
                setRoster(response.data);
                setInitialRoster(response.data);

                // Group contacts by role and update state
                if (contacts && contacts.length > 0) {
                    const groupedContacts = groupContactsByRole(contacts);
                    setContactsByRole(groupedContacts);
                }
            })
            .catch(error => {
                console.error('Error fetching roster data:', error);
            });

        // Fetch roster contacts data from the API
        axios.get('http://localhost:5000/api/rostercontact/' + rosterId)
            .then(response => {
                // Set the state with retrieved roster contacts data
            const ids =  response.data.map(rc => rc.contactId)
                setRosterContacts(ids);
            })
            .catch(error => {
                console.error('Error fetching roster contacts data:', error);
            });

        // Cleanup function to clear contactsByRole state
        return () => {
            setContactsByRole({});
        };
    }, [rosterId, contacts]); // Only re-run effect when rosterId or contacts change

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

    const handleChange = (e) => {
        const { name, value } = e.target;
        setRoster(prevState => ({
            ...prevState,
            [name]: value
        }));
    };

    const handleSubmit = () => {
        // Logic to submit updated roster data
        axios.put(`http://localhost:5000/api/roster/${rosterId}`, roster)
            .then(response => {
                console.log('Roster updated successfully:', response.data);
                // Reload the page to reflect changes
                window.location.reload();
            })
            .catch(error => {
                console.error('Error updating roster:', error);
                // Optionally, you can handle errors here, such as displaying an error message to the user.
            });
    };

    const handleCheckboxChange = (contactID) => {
        // Add or remove contactID from selectedContacts based on checkbox change
        setSelectedContacts(prevSelectedContacts => {
            if (prevSelectedContacts.includes(contactID)) {
                return prevSelectedContacts.filter(id => id !== contactID);
            } else {
                return [...prevSelectedContacts, contactID];
            }
        });
    };

    const updateRosterContacts = () => {
        console.log(selectedContacts);
        console.log(rosterContacts);
     //   Determine contacts to add and remove
        const contactsToAdd = selectedContacts.filter(contactID => !rosterContacts.includes(contactID));
        const contactsToRemove = rosterContacts.filter(contactID => !selectedContacts.includes(contactID));
        // Send request to update roster_contacts
        axios.post('http://localhost:5000/api/RosterContact/updateContacts', {
            rosterId: rosterId,
            contactsToAdd: contactsToAdd,
            contactsToRemove: contactsToRemove
        })
        .then(response => {
            console.log('Roster contacts updated successfully:', response.data);
            // Optionally, you can update the local state or perform any other actions upon successful update.
        })
        .catch(error => {
            console.error('Error updating roster contacts:', error);
            // Optionally, you can handle errors here, such as displaying an error message to the user.
        });
    };

    return (
        <div style={{ backgroundColor: 'rgb(241, 242, 242)', minHeight: '100vh', paddingTop: '64px', display: 'flex' }}>
            {/* Left Panel for Editing Roster Details */}
            <div style={{ width: '30%', padding: '20px' }}>
                <ToolBar />
                <Typography variant="h4" align="center" gutterBottom color="primary">
                    Edit Roster
                </Typography>
                {initialRoster && (
                    <div style={{ marginBottom: '20px' }}>
                        <Typography variant="h5" align="center" gutterBottom color="textSecondary">
                            Roster Name: {initialRoster.name}
                        </Typography>
                        <TextField
                            variant="outlined"
                            name="name"
                            value={roster.name}
                            onChange={handleChange}
                            fullWidth
                            margin="normal"
                        />
                    </div>
                )}
                {initialRoster && (
                    <div style={{ marginBottom: '20px' }}>
                        <Typography variant="h5" align="center" gutterBottom color="textSecondary">
                            Description: {initialRoster.description}
                        </Typography>
                        <TextField
                            variant="outlined"
                            name="description"
                            value={roster.description}
                            onChange={handleChange}
                            fullWidth
                            multiline
                            rows={4}
                            margin="normal"
                        />
                    </div>
                )}
                <Button variant="contained" color="primary" onClick={handleSubmit} fullWidth>
                    Update Roster Details
                </Button>
            </div>
            
            {/* Right Panel for Listing Contacts */}
            <div style={{ width: '70%', padding: '20px' }}>
                <Typography variant="h4" align="center" gutterBottom color="primary">
                    Contacts
                </Typography>
                <div style={{ display: 'flex', flexDirection: 'row', justifyContent: 'space-between' }}>
                    {Object.entries(contactsByRole).map(([role, contacts]) => (
                        <div key={role} style={{ marginTop: '20px', flexBasis: '30%' }}>
                            <Typography variant="h5" gutterBottom color="primary">
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
                <Button variant="contained" onClick={() => updateRosterContacts()} fullWidth style={{ marginTop: '20px' }}>
                    Set Roster Contacts
                </Button>
                <Button variant="contained" onClick={() => navigate(-1)} fullWidth style={{ marginTop: '20px' }}>
                    Go Back
                </Button>
            </div>
        </div>
    );
};
export default EditRoster;
