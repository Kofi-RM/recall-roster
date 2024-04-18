import React, { useContext, useState, useEffect } from 'react';
import { Typography, Button, Container, Grid, Paper, Tabs, Tab, Box} from '@mui/material';

import {ToolBar, MyImage} from './Miscelleneous.js'
import './Landing.css'
import { Link } from 'react-router-dom';

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
      fetch("http://localhost:5000/api/roster", {
        method: 'GET',
        // Optionally, you can pass some data in the request body
        // body: JSON.stringify({ id: contact_id }),
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
    })
    .catch(error => {
        // Handle error
        console.error('There was a problem removing the contact:', error);
    });
  }
    return (
<div>
    <ToolBar></ToolBar>
    <ul style={{ color: 'white !important' }}>
        {contacts.map(contact => (
          
          <li  key={contact.contact_id} style={{ color: 'white !important' }}>
            <span>
            <h2 className = "list">{contact.firstName + " " + contact.lastName}</h2>
          <Link to={`/editContact/${contact.contact_id}`}>
            <button>Edit</button>
            </Link>
            <button  onClick = {() => {
              console.log(typeof contact.contact_id);

              handleRemove(contact.contact_id)
            }}>  Remove</button>
          </span>
          </li>
        ))}
      </ul>
      </div>
)
        
}
export default ManageContacts;