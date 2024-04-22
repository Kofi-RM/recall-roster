import React, { useState, useEffect } from 'react';
import { Typography, Button } from '@mui/material';
import { ToolBar } from './Miscelleneous.js';
import { Link } from 'react-router-dom';
import axios from 'axios';
import useActiveRecalls from './UseActiveRecalls.js';

const RemoveRecall = ({ children }) => {
  return <span className="button">{children}</span>;
};

const ManagePrevRecalls = () => {
    const [ recallsActive, loading, error] = useActiveRecalls();

    const recallsPrev = recallsActive.filter(recall => recall.active === 0);


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
            {recallsPrev.map(recall => (
              <li key={recall.recallId} style={{ color: 'black !important', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                <div>
                  <h2 style={{ color: 'black' }} className="list">{recall.message}</h2>
                  <p>{recall.timeStarted}</p>
                </div>
                <div>
                  <Link to={`/recallStats/${recall.recallId}`}>
                    <Button size="large" variant="contained" color="primary">Check Stats</Button>
                  </Link>
                </div>
              </li>
            ))}
          </ul>
        </div>
      </div>
    </div>
  );
};

export default ManagePrevRecalls;
