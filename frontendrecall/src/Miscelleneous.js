import {  Typography, AppBar, Toolbar} from '@mui/material';

import warner from './warner.png'
import warner2 from './warner2.jpg'
import './css/Miscelleneous.css';
import { Link,  useNavigate } from 'react-router-dom';

import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';

import { useAuth } from './Auth';



export const Warner = () => {
      return (
        <div>
          <img className = "warner" src={warner} alt="Warner Robins Logo" />
        </div>
      );
  }

  export const Warner2 = () => {
    return (
      <div>
        <img className = "warner" src={warner2} alt="Warner Robins Logo" />
      </div>
    );
}
 
export const ToolBar = () => {
  const {  logout, isLoggedIn } = useAuth();

  const handleLogout = () => {
    logout();
    console.log("button press")
    // Additional logic (e.g., clearing session, redirecting, etc.) can be added here
  };
return (
    
    <div className='toolbar'>
    <AppBar >
                <Toolbar sx={{ backgroundColor: '#f1f1f1;' }}>
                    <Typography className = "toolbarName" variant="h6">Warner Robins Air Force Base</Typography>
                    {/* <Button color="inherit">Login</Button> */}
                   
                    {isLoggedIn &&  <span className = "button">
  <Link style= {{color: '#1c2347', fontSize: '1.2rem', fontFamily: '"Orbitron", sans-serif', letterSpacing: '1px'}}to="/" onClick={handleLogout}>Logout</Link>
</span>} 

{isLoggedIn &&  <span className = "button">
  <Link style= {{color: '#1c2347', fontSize: '1.2rem', fontFamily: '"Orbitron", sans-serif', letterSpacing: '1px'}}to="/Landing" >Home</Link>
</span>}
{/* Removes logout button if the user hasnt logged in yet*/}

                </Toolbar>
             
            </AppBar>
            </div>
)}

export const Footer = () => {

  return (
    <footer className="footer">
    <div className="footer-content">
      <span> Warner Robins Recall Roster 2024©</span>
      <Link to="/contact" className="footer-link">Contact Us</Link>
    </div>
  </footer>
  )
}
   


