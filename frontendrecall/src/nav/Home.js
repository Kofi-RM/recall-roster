import { ToolBar } from "../Miscelleneous.js";
import HomeBanner from "../components/HomeBanner.js";
import { Typography, Button, Container, Grid, Paper, Tabs, Tab, Box, Toolbar, TextField } from '@mui/material';
import { useNavigate } from "react-router-dom";
import { createTheme, ThemeProvider } from '@mui/material/styles';
import { NavyButton } from "../components/Buttons.js";
import { Footer } from "../Miscelleneous.js";
const theme = createTheme({
  palette: {
    custom: {
      main: '#000',
      contrastText: '#f1f1f1',
    },
  },
});

const Home = () => {
  const navigate = useNavigate();

  const handleLogin = () => {
   navigate("/login")
  }
    return (
      
        <div>
        <ToolBar></ToolBar>
      <HomeBanner></HomeBanner>
      <NavyButton onClick ={handleLogin} width = {'20%'}>Login</NavyButton>
      <Footer></Footer>
        </div>
       
    )
}

export default Home;