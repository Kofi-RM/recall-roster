import { Button, Typography, AppBar, Toolbar, Container, TextField } from '@mui/material';
import { styled } from '@mui/material/styles';
import warner from './warner.png'
import './App.css';

const useStyles = styled({
  container: {
    display: 'flex',
    justifyContent: 'center',
    alignItems: 'center',
    height: '100vh', // Adjust this height as needed for your layout
  }
});

export const MyImage = () => {
      return (
        <div>
          
          <img className = "warner" src={warner} alt="Warner Robins Logo" />
        </div>
      );
    }

  
export const ToolBar = () => {
return (
    <div className='toolbar'>
    <AppBar >
                <Toolbar>
                    <Typography variant="h6">Warner Robins Air Force Base</Typography>
                    {/* <Button color="inherit">Login</Button> */}
                </Toolbar>
            </AppBar>
            </div>
)

}

const loginComponent = () => {

   

    return (
      <div>
            <ToolBar></ToolBar>

            <Container>
                <Typography variant="h4" align="center" gutterBottom>
                    Warner Robins Recall Roster
                </Typography>

                <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', }}>
                    <Typography variant="h4" align="center" gutterBottom>
                        Username
                    </Typography>
                    <TextField label="Username" variant="outlined" sx={{ textAlign: 'center' }} InputProps = {{style:{ borderColor: 'white'}}} />
                </div>
                <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', }}>
                    <Typography variant="h4" align="center" gutterBottom>
                        Password
                    </Typography>
                    <TextField label="Password" variant="outlined" sx={{ textAlign: 'center' }} />
                </div>

                <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                    <Button variant="contained" color="primary">
                        Login
                    </Button>
                </div>
                <MyImage />
                {/* Other content */}
            </Container>
        </div>
        
            
    );
  };

  export default loginComponent;