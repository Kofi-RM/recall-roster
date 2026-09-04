import React from 'react';
import { Button } from '@mui/material';
import { Link } from 'react-router-dom';
import { useAuth } from '../Auth';
import picture from '../images/AF1.avif';
import '../css/HomeBanner.css';

const features = [
  ['01', 'Organize your people', 'Keep staff contact details and custom rosters together, ready when you need them.'],
  ['02', 'Coordinate a recall', 'Choose a roster, prepare your message, and start reaching your team.'],
  ['03', 'Follow every response', 'Review recall progress and revisit previous recalls in one workspace.'],
];

const HomeBanner = () => {
  const { isLoggedIn } = useAuth();
  return (
    <div className="rr-home">
      <section className="rr-hero" aria-labelledby="hero-title">
        <div className="rr-hero-copy">
          <p className="rr-eyebrow">WARNER ROBINS · RECALL ROSTER</p>
          <h1 id="hero-title">Stay connected.<br /><span>Be ready.</span></h1>
          <p className="rr-intro">Your people. Your rosters. One place to coordinate a recall and keep your team informed.</p>
          <Button component={Link} to={isLoggedIn ? '/landing' : '/login'} variant="contained" size="large" sx={{ bgcolor: '#e8b85b', color: '#14243b', fontWeight: 700, px: 4, py: 1.5, '&:hover': { bgcolor: '#f4cd81' } }}>
            {isLoggedIn ? 'Open dashboard' : 'Officer login'} <span aria-hidden="true">&nbsp; →</span>
          </Button>
          <p className="rr-hero-note">Roster management. Recall coordination. Response tracking.</p>
        </div>
        <figure className="rr-hero-image">
          <img src={picture} alt="Air Force personnel beside an aircraft on the flight line" />
          <figcaption>CONNECTED PEOPLE. COORDINATED RESPONSE.</figcaption>
        </figure>
      </section>
      <section className="rr-features" aria-labelledby="features-title">
        <div className="rr-section-heading"><p className="rr-eyebrow">BUILT AROUND YOUR TEAM</p><h2 id="features-title">From roster to response.</h2></div>
        <div className="rr-feature-grid">
          {features.map(([number, title, description]) => (
            <article className="rr-feature" key={number}><span className="rr-feature-number">{number}</span><h3>{title}</h3><p>{description}</p></article>
          ))}
        </div>
      </section>
    </div>
  );
};
export default HomeBanner;
