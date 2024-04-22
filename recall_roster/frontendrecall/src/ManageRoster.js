import React, { useContext, useState, useEffect } from 'react';
import { Typography, Button, Container, Grid, Paper, Tabs, Tab, Box} from '@mui/material';

import {ToolBar, MyImage} from './Miscelleneous.js'
import './ManageContacts.js'
import { Link } from 'react-router-dom';

import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import axios from 'axios';
import useRoster from './UseRoster.js'; // Adjust the path as needed

export const RemoveContact = ({children}) => {
  

  return (

      <span className = "button" >{children}</span>
  )
      
  
}

  
const ManageRoster = () =>  {
    const { rosters, loading, error } = useRoster();
    

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
        console.error('There was a problem removing the roster:', error);
    });
  }
    return (
<div>
    <ToolBar></ToolBar>
    <div className="contact-list-container">
<div className="contact-list-section">
      <h1>Roster List</h1>
      <hr></hr>
      <ul className="contact-list">
        {rosters.map(roster => (
          
          <li  key={roster.rosterId} style={{ color: 'black !important' }}>
            <span style={{ color: 'black' }}>
            <h2 style={{ color: 'black' }} className = "list">{roster.name }</h2>
            <Link to={`/editRoster/${roster.rosterId}`}>
                <Button size="large" variant="contained" color="primary">Edit</Button>
              </Link>
              <Button size="large" variant="contained" style={{ backgroundColor: 'red', color: 'white' }}  onClick={() => handleRemove(roster.rosterId)}>
                Remove
              </Button>
          </span>
          </li>
        ))}
      </ul>
     
      </div>
      </div>
    </div>
      
)
        
}
export default ManageRoster;