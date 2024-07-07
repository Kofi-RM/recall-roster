import logo from './logo.svg';


import React, { useState, useEffect,  createContext, useContext, Component } from 'react';
import { BrowserRouter as Router, Route, Routes, Link } from 'react-router-dom';
import { Button, Typography, AppBar, Toolbar, Container, TextField } from '@mui/material';
import 'bootstrap/dist/css/bootstrap.min.css';
import LoginPage from './LoginPage.js';
import LandingPage from './Landing.js';
import { AuthProvider } from './Auth';
import ManageRoster from './ManageRoster';
import InsertContact from './InsertContact.js';
import ManageContacts from './ManageContacts.js'
import axios from 'axios';
import EditContact from './EditContact.js';
import RecallStats from './RecallStats.js';
import EditRoster from './EditRoster.js';
import StartRecall from './StartRecall.js';
import CreateRoster from './CreateRoster.js';
import Home from './Home.js';

function App() {
  
  return (
    <AuthProvider>
    <Router>
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/login" element={<LoginPage/>} />
          <Route path="/landing" element={<LandingPage />} />
          <Route path="/manageContacts" element = {<ManageContacts/>} />
          
          <Route path="/addContact" element = {<InsertContact/>} />
          <Route path="/editContact/:contactId" element = {<EditContact/>} />
          <Route path="/manageRoster" element = {<ManageRoster/>} />
          <Route path="/editRoster/:rosterId" element = {<EditRoster/>} />
          <Route path="/insertContact" element = {<InsertContact/>} />
          <Route path="/recallStats/:recallId" element = {<RecallStats/>} />
          <Route path="/startRecall" element = {<StartRecall/>} />
          <Route path="/createRoster" element = {<CreateRoster/>} />


           {/* <Route path="/createRoster" element = {<CreateRoster/>}/> */}
          {/* Add more routes as needed */}
        </Routes>
      </Router>
      </AuthProvider>
  );
  }


export default App;


