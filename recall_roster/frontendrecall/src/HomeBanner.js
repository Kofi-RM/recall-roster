// FullWidthImage.js
import React from 'react';
import { Typography, Button, Container, Grid, Paper, Tabs, Tab, Box, Toolbar, TextField } from '@mui/material';
import { Link, useNavigate } from 'react-router-dom';
import picture from "../src/images/AF1.avif"
 import './css/HomeBanner.css';
import { Footer } from './Miscelleneous';

const HomeBanner = () => {


  return (
    <div>
    <div className = "flex-container">
    <div className="container">
      <div className = "picture">
      <img src={picture} alt="Air force employees line up in front of a plane" className="bg-picture" />
    </div>
    </div>
        
      
     <div className='tagline'>
     <div>
        <h2>Inform and Monitor</h2>
        
        <br></br>
        <h5>Keep your entire team safe and in the loop with our
           easy and responsive Recall Roster</h5>
       
        </div>

        </div>
   

    {/* <div style={{ marginTop: '20px', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
          <Button size="large" variant="contained" color="primary" onClick={ handleLogin}>
            Login
          </Button>
        </div> */}
    </div>
   
    </div>
  );
};

export default HomeBanner;
