import React, { useState, useEffect } from 'react';
import { BrowserRouter as Router, Route, Routes, Link } from 'react-router-dom';
import { Button, Typography, AppBar, Toolbar, Container, TextField } from '@mui/material';
import './App.css';
import 'bootstrap/dist/css/bootstrap.min.css';
import LoginPage from './LoginPage';
import LandingComponent from './Landing';

import axios from 'axios';

const About = () => <h2>About Page</h2>;


function Password() {
  const [email, setEmail] = useState('');
  const submit = (e) => {
    e.preventDefault();
    console.log("submit")
  }
  
  return  (
    <div>
  <h2>Password Recovery</h2>
  <form onSubmit={submit}>
          <div>
            <label>Email:</label>
            <input
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
            />
            </div>
            </form>
  </div>
  )
}



function App() {
   useEffect(() => {
    // Axios GET request to your .NET Core backend API endpoint
    axios.get('https://localhost:5000/api/contact')
      .then(response => {
        console.log('Response:', response.data);
        // Do something with the response data
      })
      .catch(error => {
        console.error('Error:', error);
      });
  }, []); // Empty dependency array ensures that this effect runs only once, when the component mounts
  return (
  <Router>
  <Routes>
    <Route path="/" element={<Home/>} />
    <Route path="/login" element={<LoginPage/>} />
    <Route path="/landing" element={<LandingComponent/>} />
    
    {/* Add more routes as needed */}
  </Routes>
</Router>
)


 // return LoginPage()
};

export default App;