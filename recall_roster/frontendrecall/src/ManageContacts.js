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
      <div className="contact-list-section">
      <ul className="contact-list">
        {contacts.map(contact => (
          <li className="contact-item" key={contact.contactID}>
            <div className="contact-details">
              <h2>{contact.firstName} {contact.lastName}</h2>
              <p>Email: {contact.email}</p>
              <p>Phone: {contact.phoneNumber}</p>
            </div>
            <div className="contact-actions">
              <Link to={`/editContact/${contact.contactID}`}>
                <button className="edit-button">Edit</button>
              </Link>
              <button className="remove-button" onClick={() => handleRemove(contact.contactID)}>
                Remove
              </button>
            </div>
          </li>
        ))}
      </ul>
      <Link to= {'/insertContact'}> <button>Add a Contact</button></Link>
     
       <button onClick={() => window.history.back()}>Go Back</button>
    </div>
    </div>
    
      </div>
      )
        }
export default ManageContacts;