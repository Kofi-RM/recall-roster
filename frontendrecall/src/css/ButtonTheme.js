import { createTheme, ThemeProvider } from '@mui/material/styles';

export const NavyTheme = createTheme({
    palette: {
      custom: {
        main: '#1c2347',
         light: "#003366",
      dark: "#001020",
        contrastText: '#f1f1f1',
      },
    },

    components: {
    MuiButton: {
      styleOverrides: {
        containedCustom: {
          backgroundColor: "#001f3f",
          color: "#fff",
        },
      },
    },
  }
  });
  
