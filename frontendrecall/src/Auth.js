import React, { useState, useContext, useEffect } from "react";
import { jwtDecode } from "jwt-decode";

const AuthContext = React.createContext();

export const AuthProvider = ({ children }) => {
  const [token, setToken] = useState(null);
  const [user, setUser] = useState(null);

  // Load token on refresh
  useEffect(() => {
    const storedToken = localStorage.getItem("recallToken");

    if (storedToken) {
      try {
        const decoded = jwtDecode(storedToken);

        setToken(storedToken);
        setUser(decoded);
      } catch (err) {
        console.error("Invalid token");
        localStorage.removeItem("recallToken");
      }
    }
  }, []);

  const login = (jwtToken) => {
    localStorage.setItem("recallToken", jwtToken);

    const decoded = jwtDecode(jwtToken);

    setToken(jwtToken);
    setUser(decoded);
  };

  const logout = () => {
    localStorage.removeItem("recallToken");
    setToken(null);
    setUser(null);
  };

  const isLoggedIn = !!token;

  return (
    <AuthContext.Provider
      value={{
        token,
        user,
        isLoggedIn,
        login,
        logout
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => useContext(AuthContext);