import React, { useState, useEffect } from 'react';
import { Tabs, Tab, Button } from '@mui/material';
import { Link } from 'react-router-dom';
import useContacts from './UseContacts.js'; // Adjust the path as needed
import { NavyButton } from './Buttons.js';
import './css/ItemRows.css';

const EditableRow = ({ item, onSave }) => {
  const [editMode, setEditMode] = useState(false);
  const [editedItem, setEditedItem] = useState(item);

  useEffect(() => {
    setEditedItem(item);
  }, [item]);

  const handleToggleEditMode = () => {
    setEditMode(!editMode);
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setEditedItem((prevState) => ({
      ...prevState,
      [name]: value,
    }));
  };

  const handleSave = () => {
    onSave(editedItem);
    setEditMode(false); // Exit edit mode after saving
  };

  return (
    <tr>
      {editMode ? (
        <>
          <td>
            <input
              type="text"
              name="name"
              value={`${editedItem.firstName} ${editedItem.lastName}`}
              onChange={handleChange}
            />
          </td>
          <td>
            <input
              type="text"
              name="phone"
              value={editedItem.phoneNumber}
              onChange={handleChange}
            />
          </td>
          <td>
            <select name="rank" value={editedItem.rank} onChange={handleChange}>
              <option value="">Select Option</option>
              <option value="Employee">Employee</option>
              <option value="Element Chief">Element Chief</option>
              <option value="Flight Chief">Flight Chief</option>
              <option value="Squadron Director">Squadron Director</option>
            </select>
          </td>
          <td>
            <NavyButton onClick={handleSave}>Save</NavyButton>
            <NavyButton onClick={handleToggleEditMode}>Cancel</NavyButton>
          </td>
        </>
      ) : (
        <>
          <td>{`${editedItem.firstName} ${editedItem.lastName}`}</td>
          <td>{editedItem.phoneNumber}</td>
          <td>{editedItem.rank}</td>
          <td>
            <NavyButton onClick={handleToggleEditMode}>Edit</NavyButton>
          </td>
        </>
      )}
    </tr>
  );
};

const ManageContacts = () => {
  const { contacts, loading, error } = useContacts();
  const [tabValue, setTabValue] = useState(0); // State to track the active tab index

  const handleSaveItem = (updatedItem) => {
    console.log('Saving item:', updatedItem);
    // Implement saving logic here, e.g., API call to update the item
    // Update your data source (not shown in this example)
  };

  // Define roles for each tab
  const roles = ['All', 'Employee', 'Element Chief', 'Flight Chief', 'Squadron Director'];

  // Filter contacts based on the selected tab value (role)
  const filteredContacts = contacts.filter((contact) => {
    const role = roles[tabValue];
    return role === 'All' || contact.rank === role;
  });

  if (loading) {
    return <div>Loading...</div>;
  }

  if (error) {
    return <div>Error: {error.message}</div>;
  }

  return (
    <div>
      <div className="contact-list-container">
        <h1>Contact List</h1>
        <Tabs value={tabValue} onChange={(event, newValue) => setTabValue(newValue)}>
          {roles.map((role, index) => (
            <Tab key={index} label={role} />
          ))}
        </Tabs>
        <div className="contact-list-section">
          <table>
            <thead>
              <tr>
                <th>Name</th>
                <th>Phone</th>
                <th>Role</th>
                <th>Action</th>
              </tr>
            </thead>
            <tbody>
              {filteredContacts.map((item) => (
                <EditableRow key={item.id} item={item} onSave={handleSaveItem} />
              ))}
            </tbody>
          </table>
          <div className="contact-list-buttons">
            <Link to="/insertContact">
              <NavyButton size="large" variant="contained" color="primary">
                Add a Contact
              </NavyButton>
            </Link>
          </div>
        </div>
      </div>
    </div>
  );
};

export default ManageContacts;
