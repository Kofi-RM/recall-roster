import { Box, TextField, Button, Checkbox, FormControlLabel, Grid, Paper, Typography } from '@mui/material';
import { styled } from '@mui/material/styles';
import warner from './warner.png'
import './css/LoginPage.css';
import { Link, useNavigate } from 'react-router-dom';
import axios from 'axios';
import React, { useState, useEffect, createContext, useContext } from 'react';
import { ToolBar, MyImage, Warner, Footer } from './Miscelleneous.js';
import { useAuth } from './Auth.js'
import { NavyButton } from './Buttons.js';


const LoginPage = () => {

  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [rememberMe, setRememberMe] = useState(false);
  // set email and password 
  const { login, loggedIn } = useAuth();
  // load login varaibles from Global instances

  const navigate = useNavigate();


  useEffect(() => {
    if (loggedIn) {
      console.log("well done");
      console.log(loggedIn);

    } else {
      console.log("logged out")
    }
  }, [loggedIn]); // Run the effect whenever the loggedIn state changes

  const handleLogin = (e) => {
    e.preventDefault();
console.log("tryna login");

    const loginData = {
      Email: email,
      Password: password
    };
    console.log(loginData);

    axios.post("http://localhost:5000/api/login", loginData)
      .then(response => {
        // Handle successful login
        console.log('Login successful:', response.data);
        login(); // Call your login function from the useAuth hook
        navigate("/landing"); // Redirect to landing page
      })
      .catch(error => {
        // Handle login failure
        console.error('Login failed:', error.response.data);
        // Show error message or handle the error as needed
      });
  };


  const handleLogout = () => {

    // Additional logic (e.g., clearing session, redirecting, etc.) can be added here
  };

  if (loggedIn) {
    return (
      <div>
        <h1>Welcome!</h1>
        <button onClick={handleLogout}>Logout</button>
      </div>
    );
  }

  return (
    <div className="background">
      <ToolBar ></ToolBar>

      <Grid container justifyContent="center" alignItems="center" height="100vh">
      <Grid item>
        <Paper elevation={3} sx={{ p: 4, maxWidth: 400, width: '100%' }}>
          <Typography variant="h5" gutterBottom align="center">
            Officer Login
          </Typography>
       
            <TextField
              label="Email"
              variant="outlined"
              fullWidth
              margin="normal"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
            />
            <TextField
              label="Password"
              type="password"
              variant="outlined"
              fullWidth
              margin="normal"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
            />
            <FormControlLabel
              control={<Checkbox checked={rememberMe} onChange={(e) => setRememberMe(e.target.checked)} />}
              label="Remember me"
            />
            <NavyButton onClick = {handleLogin} width= {'20%'} type="submit" variant="contained" fullWidth sx={{ mt: 2 }}>
              Login
            </NavyButton>
         
          <Box mt={2} textAlign="center">
            <Link to="/forgot-password">Forgot Password?</Link>
          </Box>
        </Paper>
      </Grid>
    </Grid>
  ;
  <Footer></Footer>
    </div>

  );
};

export default LoginPage;