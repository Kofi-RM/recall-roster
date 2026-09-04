import React, { useState } from 'react';
import { Tabs, Tab, useMediaQuery } from '@mui/material';
import { ToolBar } from './Miscelleneous';
import ManageContacts from './ManageContacts';
import ManageRoster from './ManageRoster';
import ManageActiveRecalls from './ManageActiveRecall';
import ManagePrevRecalls from './ManagePrevRecalls';
import './css/Dashboard.css';

const sections = [
  { label: 'Active Recalls', description: 'Coordinate your current recalls and follow your team’s responses.', component: ManageActiveRecalls },
  { label: 'Recall History', description: 'Revisit previous recalls and their results.', component: ManagePrevRecalls },
  { label: 'View Rosters', description: 'Organize the teams you need to reach.', component: ManageRoster },
  { label: 'View Staff', description: 'Keep your staff directory and contact details up to date.', component: ManageContacts },
];

const LandingPage = () => {
  const [selectedTab, setSelectedTab] = useState(0);
  const compact = useMediaQuery('(max-width:900px)');
  const Content = sections[selectedTab].component;
  return (
    <div className="rr-dashboard">
      <ToolBar />
      <div className="rr-dashboard-layout">
        <aside className="rr-sidebar">
          <p className="rr-eyebrow">WORKSPACE</p>
          <Tabs orientation={compact ? 'horizontal' : 'vertical'} variant="scrollable" scrollButtons="auto"
            value={selectedTab} onChange={(_, value) => setSelectedTab(value)} aria-label="Workspace sections">
            {sections.map((section, index) => <Tab key={section.label} label={section.label} id={`workspace-tab-${index}`} aria-controls={`workspace-panel-${index}`} />)}
          </Tabs>
          <div className="rr-sidebar-note">Recall Roster<span>Keep your team within reach.</span></div>
        </aside>
        <main id="main-content" className="rr-dashboard-main">
          <header className="rr-dashboard-heading"><p className="rr-eyebrow">RECALL MANAGEMENT</p><h1>{sections[selectedTab].label}</h1><p>{sections[selectedTab].description}</p></header>
          <section className="rr-workspace-panel" role="tabpanel" id={`workspace-panel-${selectedTab}`} aria-labelledby={`workspace-tab-${selectedTab}`} tabIndex={0}>
            <Content />
          </section>
        </main>
      </div>
    </div>
  );
};
export default LandingPage;
