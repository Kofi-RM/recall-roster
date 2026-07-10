import { Box, TextField, Button, Checkbox, FormControlLabel, Grid, Paper, Typography } from '@mui/material';

import './css/LoginPage.css';
import { Link, useNavigate } from 'react-router-dom';
import axios from 'axios';
import React, { useState} from 'react';
import { ToolBar, Footer } from './Miscelleneous.js';
import { useAuth } from './Auth.js'
import { NavyButton } from './components/Buttons.js';


const LoginPage = () => {

  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [rememberMe, setRememberMe] = useState(false);
  // set email and password 
  const { login, logout } = useAuth();
  // load login varaibles from Global instances

  const navigate = useNavigate();


  const handleLogin = async (e) => {
    e.preventDefault();
console.log("tryna login");

    const loginData = {
      Email: email,
      Password: password
    };
    console.log(loginData);

    const res = await axios.post("http://localhost:5000/api/auth/login", {
    email,
    password
  });

  login(res.data.token);
  console.log("before nav")
 setTimeout(() => {
  navigate("/landing");
}, 0);
  console.log("after nav")
  };

 

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