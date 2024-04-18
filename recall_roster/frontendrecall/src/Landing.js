import React, { useContext, useState } from 'react';
import { Typography, Button, Container, Grid, Paper, Tabs, Tab, Box} from '@mui/material';

import {ToolBar, Warner, Warner2} from './Miscelleneous.js'
import './Landing.css'
import { DbButton } from './Buttons.js';
import { Link , useNavigate} from 'react-router-dom';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';



const LandingPage = () => {
  const [selectedTab, setSelectedTab] = useState(0);

  const userOptions = ['Active Recalls', 'Previous Recalls', 'Edit Rosters']; // Replace with actual user options
  const userDetails = {
    0: 'Active Recalls with a Status Button', // Replace with actual user details based on the option
    1: 'Former Recalls sorted by date',
    2: 'Roster Info and Edit Buttons',
  };

  const userTitle = {
    0: 'List of Active Recalls', // Replace with actual user details based on the option
    1: 'List of Former Recalls',
    2: 'Manage Roster',
  };


  const handleTabChange = (event, newValue) => {
    setSelectedTab(newValue);
  };

  const handleLogout = () => {
    // Implement logout functionality
    // For example: Clear user authentication context or perform logout API call
    
  };

  const navigate = useNavigate(); // needs to be declared outside the function!!
  const List = () => {
      navigate("/manageContacts")

  }

  const Recall = () => {
    navigate("/")
  }
  return (
    <div>
    
       <ToolBar></ToolBar>
       
    <Container className = "landing" style={{ padding: '25px',  borderRadius: 16, boxShadow: '0px 8px 16px rgba(0, 0, 0, 0.1)' }}>
     
      <div className = "colorBox1"></div>
      <div className = "colorBox2"></div>
      <Grid container spacing={2}>
        {/* Left column with buttons and tabs */}
        <Grid item xs={2}>
          <Paper>
            <Typography variant="h6" align="center" style={{ margin: '1rem 0', fontFamily: '"Roboto", sans-serif'  }}>
              User Options
            </Typography>
            <Tabs
              orientation="vertical"
              variant="scrollable"
              value={selectedTab}
              onChange={handleTabChange}
            >
              {userOptions.map((option, index) => (
                <Tab label={option} key={index} />
              ))}
            </Tabs>
          </Paper>
        </Grid>

        {/* Right column with user information */}
        <Grid item xs={8}>
          <Paper>
            <Typography variant="h6" align="center" style={{ margin: '1rem 0', fontFamily: '"Roboto", sans-serif'  }}>
            {userTitle[selectedTab]}
            </Typography>
            <Box p={2}>
              <Typography variant="body1" style={{ margin: '1rem 0', fontFamily: '"Roboto", sans-serif'  }}>{userDetails[selectedTab]}</Typography>
              <ul>
               
              </ul>
              {selectedTab === 0 && (
          <DbButton onClick={List}>View All</DbButton>
        )}
        {selectedTab === 1 && (
          <DbButton onClick={List}>View All</DbButton>
        )}
        {selectedTab === 2 && (
          <DbButton onClick={List}>Make a Roster</DbButton>
        )}

            </Box>
          </Paper>
        </Grid>
      </Grid>

      {/* Logout button 
     <Button variant="contained" color="primary" onClick={handleLogout} style={{ marginTop: '1rem' }}>
        Logout
      </Button> 
      */}

      {/* <span className = "button" >
  Button
</span> */}

<DbButton onClick={List}>List Contacts</DbButton>


<Warner/> {/* Image at the bottom of page*/}
    </Container>

    </div>
  );
};

export default LandingPage;