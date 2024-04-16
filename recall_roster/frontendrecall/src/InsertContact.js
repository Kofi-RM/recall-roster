import React, { useContext, useState } from 'react';
import { Typography, Button, Container, Grid, Paper, Tabs, Tab, Box, Toolbar, TextField, Select, MenuItem} from '@mui/material';

import {ToolBar, Warner, Warner2} from './Miscelleneous.js'
import './Landing.css'
import { DbButton } from './Buttons.js';
import { Link , useNavigate} from 'react-router-dom';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import {prevPage} from './PageNav.js';


const InsertContact = () => {
    const [firstName, setFirst] = useState('');
    const [lastName, setLast] = useState('');
    const [phoneNumber, setPhoneNumber] = useState('');
    const [selectedOption, setSelectedOption] = useState('');
   // set email and password 


   const { goBack } = prevPage();

   const handleRemove = async (id) => {
       
  
//       fetch("http://localhost:5000/api/login", {
//         method: 'POST',
//         // Optionally, you can pass some data in the request body
//         // body: JSON.stringify({ id: contactId }),
//         headers: {
         
//         }
//     })
//     .then(response => {
//         if (!response.ok) {
//             throw new Error('Network response was not ok');
//         }
//         return response.json();
//     })
//     .then(data => {
//         // Handle success
//         console.log('Contact removed:', data);
//     })
//     .catch(error => {
//         // Handle error
//         console.error('There was a problem removing the contact:', error);
//     });
   }  
return (
<div className = "background">
            <ToolBar ></ToolBar>

            <Container style={{ padding: '25px',  borderRadius: 16 }}>
               
                <Typography variant="h4" align="center" gutterBottom  className = "logoName">
                  Please enter your Contact Details
                  </Typography>
                <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', }}>
                    <span style={{ marginLeft: '13px' }}>
                    <Typography className = "login" variant="h4" align="center" gutterBottom margin='10px' padding = '20px'>
                        First Name    
                    </Typography>
                    </span>
                    <span style={{ marginLeft: '10px' }}>
                    <TextField label="First name" variant="outlined" value = {firstName} onChange={(e => setFirst(e.target.value))} sx={{ textAlign: 'center' }} InputProps = {{style:{ borderColor: 'white'}}} />
                    </span>
                </div>
                <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', }}>
                    <Typography className = "login" variant="h4" align="center" gutterBottom margin='12px' padding-right = '30px'>
                        Last Name
                    </Typography>
                    <TextField label="Last name" variant="outlined" value = {lastName} onChange={(e => setLast(e.target.value))}  sx={{ textAlign: 'center' }} margin='20px' />
                </div>

                <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', }}>
                    <Typography className = "login" variant="h4" align="center" gutterBottom margin='12px' padding-right = '30px'>
                        Phone Number
                    </Typography>
                    <TextField label="123-456-7899" variant="outlined" value = {lastName} onChange={(e => setLast(e.target.value))}  sx={{ textAlign: 'center' }} margin='20px' />
                </div>
                      {/*Onchange function is what lets you typei nto the text boxes*/ }

                      <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', marginTop: '20px' }}>
                    <Typography className = "login"variant="h4" align="center" gutterBottom margin='12px' padding-right = '30px'>Rank</Typography>
                    <Select 
                        value={selectedOption}
                        onChange={(e) => setSelectedOption(e.target.value)}
                        variant="outlined"
                        sx={{ minWidth: 200 }}
                    >
                        <MenuItem value="">Select Option</MenuItem>
                        <MenuItem value="1">Option 1</MenuItem>
                        <MenuItem value="2">Option 2</MenuItem>
                        <MenuItem value="3">Option 3</MenuItem>
                    </Select>
                </div>
                <div style={{ marginTop: '20px', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                    <Button size="large" variant="contained" color="primary" >
                        Add Contact
                    </Button>
                    <Button onClick={goBack}>Previous Page</Button>
                </div>
                <Warner />
                {/* Other content */}
            </Container>
        </div>
        )}

        export default InsertContact;