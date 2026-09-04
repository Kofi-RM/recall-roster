import { AppBar, Toolbar, Button } from '@mui/material';
import { Link } from 'react-router-dom';
import { useAuth } from './Auth';
import warner from './warner.png';
import warner2 from './warner2.jpg';
import './css/Miscelleneous.css';

export const Warner = () => <img className="warner" src={warner} alt="Warner Robins Logo" />;
export const Warner2 = () => <img className="warner" src={warner2} alt="Warner Robins Logo" />;

export const ToolBar = () => {
  const { logout, isLoggedIn } = useAuth();
  return (
    <div className="toolbar">
      <a className="rr-skip" href="#main-content">Skip to main content</a>
      <AppBar elevation={0} sx={{ bgcolor: '#fff', color: '#14243b', borderBottom: '1px solid #e1e6ed' }}>
        <Toolbar sx={{ gap: 2, minHeight: '72px !important', px: { xs: 2, md: 5 } }}>
          <Link className="rr-brand" to={isLoggedIn ? '/landing' : '/'}>
            <span className="rr-brand-mark" aria-hidden="true">RR</span>
            <span>Recall Roster<small>Warner Robins Air Force Base</small></span>
          </Link>
          <nav className="rr-header-nav" aria-label="Main navigation">
            {isLoggedIn ? <>
              <Button component={Link} to="/landing" color="inherit">Dashboard</Button>
              <Button component={Link} to="/" onClick={logout} color="inherit" variant="outlined">Log out</Button>
            </> : <Button component={Link} to="/login" color="inherit">Log in <span aria-hidden="true">&nbsp;→</span></Button>}
          </nav>
        </Toolbar>
      </AppBar>
    </div>
  );
};

export const Footer = () => (
  <footer className="rr-footer">
    <span>© {new Date().getFullYear()} Warner Robins Recall Roster</span>
    <span>Stay connected. Be ready.</span>
  </footer>
);
