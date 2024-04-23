import React, { useState } from 'react';
import { Container, Row, Col } from 'react-bootstrap';
import loginComponent from './Components';


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
function LoginPage() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [loggedIn, setLoggedIn] = useState(false);

  const handleLogin = (e) => {
    e.preventDefault();
    // Perform authentication logic here (e.g., API call, validation)

    // For demonstration purposes, let's simulate successful login with hardcoded credentials
    if (email === 'user@example.com' && password === 'password') {
      setLoggedIn(true);
      // You might redirect to another page upon successful login
      // Replace '/dashboard' with the route you want to redirect to
      // history.push('/dashboard');
    } else {
      alert('Invalid email or password');
    }
  };

  const handleLogout = () => {
    setLoggedIn(false);
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
loginComponent()
   );
};

export default LoginPage;