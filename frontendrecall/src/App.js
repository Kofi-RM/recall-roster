import logo from './logo.svg';


import React, { useState, useEffect,  createContext, useContext, Component } from 'react';
import { BrowserRouter as Router, Route, Routes, Link } from 'react-router-dom';
import { Button, Typography, AppBar, Toolbar, Container, TextField } from '@mui/material';
import 'bootstrap/dist/css/bootstrap.min.css';
import LoginPage from './LoginPage.js';
import LandingPage from './Landing.js';
import { AuthProvider } from './Auth';
import ManageRoster from './ManageRoster';
import InsertContact from "./nav/InsertContact.js"
import ManageContacts from './ManageContacts.js'
import ProtectedRoute from './ProtectedRoute.js';
import EditContact from './nav/EditContact.js';
import RecallStats from './nav/RecallStats.js';
import EditRoster from './nav/EditRoster.js';
import StartRecall from './nav/StartRecall.js';
import CreateRoster from './nav/CreateRoster.js';
import Home from './nav/Home.js';

function App() {
  
  return (
    <AuthProvider>
    <Router>
        <Routes>
           {/* PUBLIC */}
  <Route path="/" element={<Home />} />
  <Route path="/login" element={<LoginPage />} />

  {/* PROTECTED */}
  <Route
    path="/landing"
    element={
      <ProtectedRoute>
        <LandingPage />
      </ProtectedRoute>
    }
  />

  <Route
    path="/manageContacts"
    element={
      <ProtectedRoute>
        <ManageContacts />
      </ProtectedRoute>
    }
  />

  <Route
    path="/addContact"
    element={
      <ProtectedRoute>
        <InsertContact />
      </ProtectedRoute>
    }
  />

  <Route
    path="/editContact/:contactId"
    element={
      <ProtectedRoute>
        <EditContact />
      </ProtectedRoute>
    }
  />

  <Route
    path="/manageRoster"
    element={
      <ProtectedRoute>
        <ManageRoster />
      </ProtectedRoute>
    }
  />

  <Route
    path="/editRoster/:rosterId"
    element={
      <ProtectedRoute>
        <EditRoster />
      </ProtectedRoute>
    }
  />

  <Route
    path="/insertContact"
    element={
      <ProtectedRoute>
        <InsertContact />
      </ProtectedRoute>
    }
  />

  <Route
    path="/recallStats/:recallId"
    element={
      <ProtectedRoute>
        <RecallStats />
      </ProtectedRoute>
    }
  />

  <Route
    path="/startRecall"
    element={
      <ProtectedRoute>
        <StartRecall />
      </ProtectedRoute>
    }
  />

  <Route
    path="/createRoster"
    element={
      <ProtectedRoute>
        <CreateRoster />
      </ProtectedRoute>
    }
  />


           {/* <Route path="/createRoster" element = {<CreateRoster/>}/> */}
          {/* Add more routes as needed */}
        </Routes>
      </Router>
      </AuthProvider>
  );
  }


export default App;


