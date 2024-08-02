import { Button, Typography, AppBar, Toolbar, Container, TextField } from '@mui/material';

import { Link,  useNavigate } from 'react-router-dom';

import React, { useState, useEffect } from 'react';

import styled from 'styled-components';
import { createTheme, ThemeProvider } from '@mui/material/styles';
import { NavyTheme } from './ButtonTheme';

export const NavyButton = ({children, className, onClick, width, height}) => {
  return (
<ThemeProvider theme = {NavyTheme}>
  {/* <div style={{ marginTop: '20px', display: 'flex', alignItems: 'center', justifyContent: 'center' }}> */}
          <Button sx={{width: width, height: height}} size="large" variant="contained" color="custom" onClick={ onClick}>
            {children}
          </Button>
    
        </ThemeProvider>
)}
// Styled DbButton component
const StyledDbButton = styled.button`
  /* Your button styles */
  background-color: #007bff;
  color: #fff;
  padding: 8px 16px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  margin-right: 6px;
  margin-top: 5px;

  /* Hover effect */
  &:hover {
    background-color: #0056b3;
  }
`;





export const DbButton = ({ children, className, onClick }) => {

    const navigate = useNavigate();
    const dbList = () => {
  
      navigate("/manageRoster")
    }
    return (
      <StyledDbButton onClick={onClick}>{children}</StyledDbButton>
    //<span className = "button" style= {{color: "white"}} onClick={dbList}>{children}</span>
    )
  }

  // export const RemoveContact = ({children}) => {
  //   const { contacts, loading, error } = useContacts();


 

  //   return (

  //       <span className = "button" >{children}</span>
  //   )
        
    
  // }
  