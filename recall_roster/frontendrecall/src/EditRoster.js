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
        axios.put(`http://localhost:5000/api/roster/update/${rosterId}`, roster)
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
        <div style={{ backgroundColor: 'rgb(241, 242, 242' }}>
            <ToolBar />
            <Container style={{ display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
                <Typography variant="h4" align="center" gutterBottom color="black">
                    Edit this Roster
                </Typography>
                {initialRoster && (
                    <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
                        <Typography variant="h4" align="center" gutterBottom color="black">
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
                        <Typography variant="h4" align="center" gutterBottom color="black">
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
                    Update Roster Details
                </Button>
                <Button variant="contained" onClick={() => navigate(-1)} style={{ position: 'fixed', bottom: '20px', left: '20px' }}>Go Back</Button>
                 {/* Display contacts grouped by role */}
                 {Object.entries(contactsByRole).map(([role, contacts]) => (
                    <div key={role}>
                        <Typography variant="h5" gutterBottom>{role}</Typography>
                        <ul>
                            {contacts.map(contact => {
                              //  console.log('Contact:', contact);
                                return (
                                    <li key={contact.contactID} style={{ display: 'grid', gridTemplateColumns: 'auto max-content', gap: '10px' }}>
        <div>
            {contact.firstName} {contact.lastName}
        </div>
        <input
            type="checkbox"
            value={contact.contactID}
            onChange={() => handleCheckboxChange(contact.contactID)}
        />
    </li>
                                   
                                );
                            })}
                        </ul>

                       
                 {/* Display contacts grouped by role */}
                    </div>

                ))}
                 <Button variant="contained" onClick={() => updateRosterContacts()}>Set Roster Contacts</Button>
            </Container>
        </div>
    );
};

export default EditRoster;
