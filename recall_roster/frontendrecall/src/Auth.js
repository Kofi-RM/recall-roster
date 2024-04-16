// AuthContext.js

import React, { useState, useContext } from 'react';

const AuthContext = React.createContext();

export const AuthProvider = ({ children }) => {
  const [loggedIn, setLoggedIn] = useState(false);

  const login = () => {
    // Perform login logic
    setLoggedIn(true);
    
  };

  const logout = () => {
    // Perform logout logic
    setLoggedIn(false);

  };

  return (
    <AuthContext.Provider value={{ loggedIn, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  return useContext(AuthContext);
};