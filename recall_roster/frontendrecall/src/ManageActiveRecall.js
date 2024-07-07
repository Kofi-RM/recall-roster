import React, { useState, useEffect } from 'react';
import { Typography, Button } from '@mui/material';
import { ToolBar } from './Miscelleneous.js';
import { Link } from 'react-router-dom';
import axios from 'axios';
import useActiveRecalls from './UseActiveRecalls.js';
import { NavyButton } from './Buttons.js';


const ManageActiveRecalls = () => {
const [ recallsActive, loading, error] = useActiveRecalls();

const recallActive = recallsActive.filter(recall => recall.active === 1);

  if (loading) {
    return <div>Loading...</div>;
  }

  if (error) {
    return <div>Error: {error.message}</div>;
  }
  return (
    <div>
      <ToolBar />
      <div className="contact-list-container">
        <div className="contact-list-section">
          <h1>Recall List</h1>
          <hr />
          <ul className="contact-list">
            {recallActive.map(recall => (
              <li key={recall.recallId} style={{ color: 'black !important', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                <div>
                  <h2 style={{ color: 'black' }} className="list">{recall.message}</h2>
                  <p>{recall.timeStarted}</p>
                </div>
                <div>
                  <Link to={`/recallStats/${recall.recallId}`}>
                    <NavyButton size="large" variant="contained" color="primary">Check Progress</NavyButton>
                  </Link>
                </div>
              </li>
            ))}
          </ul>
        </div>
        <div  style={{ position: 'fixed', bottom: '20px', left: '250px' }}>
          <Link to={'/StartRecall'}> <NavyButton size="large" variant="contained" color="primary">Start a Recall</NavyButton></Link>
         </div>
      </div>
    </div>
  );
};

export default ManageActiveRecalls;
