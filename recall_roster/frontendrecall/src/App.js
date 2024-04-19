import logo from './logo.svg';
import './App.css';

import React, { useState, useEffect,  createContext, useContext, Component } from 'react';
import { BrowserRouter as Router, Route, Routes, Link } from 'react-router-dom';
import { Button, Typography, AppBar, Toolbar, Container, TextField } from '@mui/material';
import './App.css';
import 'bootstrap/dist/css/bootstrap.min.css';
import LoginPage from './LoginPage.js';
import LandingPage from './Landing.js';
import { AuthProvider } from './Auth';
import ManageRoster from './ManageRoster';
import InsertContact from './InsertContact.js';
import ManageContacts from './ManageContacts.js'
import axios from 'axios';
import EditContact from './EditContact.js';

import EditRoster from './EditRoster.js';


function App() {

// Step 2: Provide the context


  // useEffect(() => {
  //   // Axios GET request to your .NET Core backend API endpoint
  //   axios.get('http://localhost:5000/api/contact')
  //     .then(response => {
  //       console.log('Response:', response.data);
  //       // Do something with the response data
  //     })
  //     .catch(error => {
  //       console.error('Error:', error);
  //     });
  // }, []); // Empty dependency array ensures that this effect runs only once, when the component mounts
  
  return (
    <AuthProvider>
    <Router>
        <Routes>
          <Route path="/" element={<LoginPage />} />
          <Route path="/landing" element={<LandingPage />} />
          <Route path="/manageContacts" element = {<ManageContacts/>} />
          
          <Route path="/addContact" element = {<InsertContact/>} />
          <Route path="/editContact/:contactId" element = {<EditContact/>} />
          <Route path="/manageRoster" element = {<ManageRoster/>} />
          <Route path="/editRoster/:rosterId" element = {<EditRoster/>} />
          <Route path="/insertContact" element = {<InsertContact/>} />

           {/* <Route path="/createRoster" element = {<CreateRoster/>}/> */}
          {/* Add more routes as needed */}
        </Routes>
      </Router>
      </AuthProvider>
  );
  }


export default App;


