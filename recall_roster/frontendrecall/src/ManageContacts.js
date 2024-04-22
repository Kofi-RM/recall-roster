import React, { useContext, useState, useEffect } from 'react';
import { Typography, Button, Container, Grid, Paper, Tabs, Tab, Box} from '@mui/material';

import {ToolBar, MyImage} from './Miscelleneous.js'
import './ManageContacts.css'
import { Link } from 'react-router-dom';
import { useNavigate } from 'react-router-dom';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import axios from 'axios';
import useContacts from './UseContacts.js'; // Adjust the path as needed


export const RemoveContact = ({children}) => {
  const { contacts, loading, error } = useContacts();

  return (

      <span className = "button" >{children}</span>
  )
      
  
}

  
const ManageContacts = () =>  {
  const { contacts, loading, error } = useContacts();
  const navigate = useNavigate();
  const [tabValue, setTabValue] = useState(0); // State to track the active tab index

  // Filter contacts based on the selected tab value (role)

  // Define roles for each tab
  const roles = ['All', 'Employee', 'Element Chief', 'Flight Chief', 'Squadron Director'];

  // Function to get the role based on tab index
  const getTabRole = (index) => {
    switch (index) {
      case 0:
        return ''; // All roles
      case 1:
        return 'Employee';
      case 2:
        return 'Element Chief';
      case 3:
        return 'Flight Chief';
      case 4:
        return 'Squadron Director';
      default:
        return '';
    }
  }
  const filteredContacts = contacts.filter(contact => {
    if (tabValue === 0) {
      return true; // Include all contacts
    } else {
      return contact.role === getTabRole(tabValue);
    }
  });

    if (loading) {
        console.log("d loading")
        return <div>Loading...</div>;
      }
    
      if (error) {
        return <div>Error: {error.message}</div>;
      }
    const handleEdit = async (id) => {

    }
      const handleRemove = async (id) => {
       
    //     try {
    //         const response = await axios.get(`http://localhost:5000/api/contact/remove/${id}`);
    //         console.log(response.data); // Log the response from the server
    //     } catch (error) {
    //         console.error('Error:', error);
    //     }
    // };
    // return (
      fetch("http://localhost:5000/api/contact/remove/" + id, {
        method: 'PUT',
        // Optionally, you can pass some data in the request body
        // body: JSON.stringify({ id: contactID }),
        headers: {
         
        }
    })
    .then(response => {
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
        return response.json();
    })
    .then(data => {
        // Handle success
        console.log('Contact removed:', data);
        navigate("/manageContacts");

    })
    .catch(error => {
        // Handle error
        console.error('There was a problem removing the contact:', error);
    });
  }
    return (
<div>
      <ToolBar></ToolBar>
      <div className="contact-list-container">
        <h1>Contact List</h1>
        <Tabs value={tabValue} onChange={(event, newValue) => setTabValue(newValue)}>
          {roles.map((role, index) => (
            <Tab key={index} label={role} />
          ))}
        </Tabs>
        <div className="contact-list-section"  style={{ position: 'relative' }}>
          <ul className="contact-list">
            {filteredContacts.map(contact => (
              <li className="contact-item" key={contact.contactID}>
                <div className="contact-details">
                  <h2>{contact.firstName} {contact.lastName}</h2>
              
                  <p>Phone: {contact.phoneNumber}</p>
                  <p>Role: {contact.role}</p>
                </div>
                <div className="contact-actions">
                  <Link to={`/editContact/${contact.contactID}`}>
                    <Button size="large" variant="contained" color="primary">Edit</Button>
                  </Link>
                  <Button size="large" variant="contained" style={{ backgroundColor: 'red', color: 'white' }} onClick={() => handleRemove(contact.contactID)}>
                    Remove
                  </Button>
                </div>
              </li>
            ))}
          </ul>
          <div  style={{ position: 'fixed', bottom: '20px', left: '250px' }}>
          <Link to={'/insertContact'}> <Button size="large" variant="contained" color="primary">Add a Contact</Button></Link>
         </div>
        </div>
      </div>
    </div>
      )
        }
export default ManageContacts;