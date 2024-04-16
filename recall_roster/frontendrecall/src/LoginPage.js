import { Typography, Button, Container, Grid, Paper, Tabs, Tab, Box, Toolbar, TextField } from '@mui/material';
import { styled } from '@mui/material/styles';
import warner from './warner.png'
import './LoginPage.css';
import { Link, useNavigate } from 'react-router-dom';
import axios from 'axios';
import React, { useState, useEffect, createContext, useContext } from 'react';
import { ToolBar, MyImage, Warner } from './Miscelleneous.js';
import { useAuth } from './Auth.js'



// function Password() {
//   const [Email, setEmail] = useState('');
//   const submit = (e) => {
//     e.preventDefault();
//     console.log("submit")
//   }

//   return (
//     <div>
//       <h2>Password Recovery</h2>
//       <form onSubmit={submit}>
//         <div>
//           <label>Email:</label>
//           <input
//             type="email"
//             value={Email}
//             onChange={(e) => setEmail(e.target.value)}
//             required
//           />
//         </div>
//       </form>
//     </div>
//   )
// }

const LoginPage = () => {

  const [Email, setEmail] = useState('');
  const [Password, setPassword] = useState('');
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

    // if (email === 'a' && password === 'p') {
    //   login()
    //   navigate("/landing")
    // } else {
    //   alert('Invalid email or password');
    // }

    const loginData = {
      Email: Email,
      Password: Password
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
logg
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

      <Container style={{ padding: '25px', borderRadius: 16 }}>
        <Typography variant="h4" align="center" gutterBottom className="logoName">
          Warner Robins Recall Roster
        </Typography>
        <Typography variant="h4" align="center" gutterBottom className="logoName">
          Login
        </Typography>
        <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', }}>
          <span style={{ marginLeft: '13px' }}>
            <Typography className="login" variant="h4" align="center" gutterBottom margin='10px' padding='20px'>
              Email
            </Typography>
          </span>
          <span style={{ marginLeft: '15px' }}>
            <TextField label="Email" variant="outlined" value={Email} onChange={(e => setEmail(e.target.value))} sx={{ textAlign: 'center' }} InputProps={{ style: { borderColor: 'white' } }} />
          </span>
        </div>
        <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', }}>
          <Typography className="login" variant="h4" align="center" gutterBottom margin='12px' padding-right='30px'>
            Password
          </Typography>
          <TextField label="Password" variant="outlined" value={Password} onChange={(e => setPassword(e.target.value))} sx={{ textAlign: 'center' }} margin='20px' />
        </div>
        {/*Onchange function is what lets you typei nto the text boxes*/}
        <div style={{ marginTop: '20px', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
          <Button size="large" variant="contained" color="primary" onClick={handleLogin}>
            Login
          </Button>
        </div>
        <Warner />
        {/* Other content */}
      </Container>
    </div>

  );
};

export default LoginPage;