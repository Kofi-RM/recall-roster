import React, { useState } from 'react';
import { Typography, Paper, Tabs, Tab, Box, Grid, Button } from '@mui/material';

import { DbButton } from './Buttons.js';
import { ToolBar } from './Miscelleneous.js';
import ManageContacts from './ManageContacts.js';
import ManageRoster from './ManageRoster.js';
import ManageActiveRecalls from './ManageActiveRecall.js';
import ManagePrevRecalls from './ManagePrevRecalls.js';
const LandingPage = () => {
  const [selectedTab, setSelectedTab] = useState(0);

  const userOptions = ['Active Recalls', 'Recall History', 'Custom Rosters', 'View Staff']; // Add 'Add Contacts' option
  const userDetails = {
    0: 'Active Recalls with a Status Button',
    1: 'Former Recalls sorted by date',
    2: 'Roster Info and Edit Buttons',
    3: 'Add new contacts to the system', // Details for 'Add Contacts' option
  };

  const userTitle = {
    0: 'List of Active Recalls',
    1: 'List of Former Recalls',
    2: 'Manage Roster',
    3: 'Add Contacts', // Title for 'Add Contacts' option
  };

  const handleTabChange = (event, newValue) => {
    setSelectedTab(newValue);
  };

  const renderTabContent = () => {
    switch (selectedTab) {
      case 0:
        return <ManageActiveRecalls/>
      case 1:
        return <ManagePrevRecalls/>
      case 2:
        return <ManageRoster/>
        case 3:
        return <ManageContacts/>// Render content for 'Add Contacts' option
      default:
        return <div>No additional content for this tab</div>; // Default message for other tabs
    }
  };

  return (
    <div>
      <ToolBar></ToolBar>
      <div style={{paddingTop:'20px', marginTop: '-20px', display: 'flex', height: '100vh' }}> {/* Add marginTop: '-20px' to remove the gap */}
        {/* Left Sidebar */}
        <Paper elevation={3} style={{display: 'block', position: 'fixed', width: '15%', backgroundColor: '#f0f0f0', height: '100%', overflowY: 'auto' }}>
          <Typography variant="h6" align="center" style={{ margin: '1rem 0' }}>Navigation</Typography>
          <Tabs
            orientation="vertical"
            variant="scrollable"
            value={selectedTab}
            onChange={handleTabChange}
            textColor="primary"
            indicatorColor="primary"
            style={{ marginTop: '20px' }}
          >
            {userOptions.map((option, index) => (
              <Tab sx = {{color:'#1c2347'}}label={option} key={index} />
            ))}
          </Tabs>
        </Paper>

        {/* Right Content */}
        <Box style={{marginLeft: '15%', width:'85%', marginTop: '-40px',flexGrow: 1, paddingRight: '25px', paddingLeft: '25px', backgroundColor: '#fff', height: '100vh'  }}>
          <Grid container spacing={2}>
            <Grid item xs={12}>
              {/* <Typography variant="h4" gutterBottom>{userTitle[selectedTab]}</Typography> */}
            </Grid>
            <Grid item xs={12}>
              {/* <Typography variant="body1">{userDetails[selectedTab]}</Typography> */}
            </Grid>
            <Grid item xs={12}>
              {/* Placeholder for the content related to the selected tab */}
              {renderTabContent()}
            </Grid>
          </Grid>
          
        </Box>
      </div>
    </div>
  );
};

export default LandingPage;
