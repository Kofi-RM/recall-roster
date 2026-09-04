import { Button} from '@mui/material';

import { useNavigate } from 'react-router-dom';

import React from 'react';

import styled from 'styled-components';


export const NavyButton = ({children, className, onClick, sx, ...props}) => {
  return (

 <>
          <Button  size="large" variant="contained" sx={{
        backgroundColor: "custom.main",
        color: "custom.contrastText",
        "&:hover": {
          backgroundColor: "custom.dark",
        }, ...sx,
      }} 
      onClick={ onClick} {...props}>
            {children}
          </Button>
  </>  
       
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
  