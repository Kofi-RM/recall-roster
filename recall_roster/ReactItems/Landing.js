import React, { useContext, useState } from 'react';
import { Typography, Button, Container, Grid, Paper, Tabs, Tab, Box} from '@mui/material';
import { Row, Col } from 'react-bootstrap';
import {ToolBar, MyImage} from './Components.js'
import './Landing.css'

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

  return (
    <div>
       <ToolBar></ToolBar>
    <Container className = "landing" style={{ marginTop: '2rem' }}>
     
      <div className = "colorBox1"></div>
      <div className = "colorBox2"></div>
      <Grid container spacing={2}>
        {/* Left column with buttons and tabs */}
        <Grid item xs={4}>
          <Paper>
            <Typography variant="h6" align="center" style={{ margin: '1rem 0' }}>
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
            <Typography variant="h6" align="center" style={{ margin: '1rem 0' }}>
            {userTitle[selectedTab]}
            </Typography>
            <Box p={2}>
              <Typography variant="body1">{userDetails[selectedTab]}</Typography>
            </Box>
          </Paper>
        </Grid>
      </Grid>

      {/* Logout button */}
      <Button variant="contained" color="primary" onClick={handleLogout} style={{ marginTop: '1rem' }}>
        Logout
      </Button>
      <MyImage></MyImage>
    </Container>

    </div>
  );
};

export default LandingPage;