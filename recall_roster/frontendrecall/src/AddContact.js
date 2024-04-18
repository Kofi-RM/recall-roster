import React, { useState } from 'react';
import axios from 'axios'; // Import Axios for making HTTP requests


function AddContact() {
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [phoneNumber, setPhoneNumber] = useState('');
  const [selectedOption, setSelectedOption] = useState('');

  const handleSubmit = (event) => {
    event.preventDefault();

    // Make a POST request to the backend API to add a new contact
    axios.post('http://localhost:5000/api/Contact', {
      firstName,
      lastName,
      phoneNumber,
      active: 1,
      Role: selectedOption
    })
    .then(response => {
      console.log('Contact added successfully:', response.data);
      // Clear form fields after successful submission
      setFirstName('');
      setLastName('');
      setPhoneNumber('');
      setSelectedOption('');
    })
    .catch(error => {
        console.log(firstName + lastName + phoneNumber);
      console.error('Error adding contact:', error);
    });
  };

  return (
    <div>
      <h2>Add Contact</h2>
      <form onSubmit={handleSubmit}>
        <div>
          <label htmlFor="firstName">First Name:</label>
          <input type="text" id="firstName" value={firstName} onChange={(e) => setFirstName(e.target.value)} />
        </div>
        <div>
          <label htmlFor="lastName">Last Name:</label>
          <input type="text" id="lastName" value={lastName} onChange={(e) => setLastName(e.target.value)} />
        </div>
        <div>
          <label htmlFor="phoneNumber">Phone Number:</label>
          <input type="text" id="phoneNumber" value={phoneNumber} onChange={(e) => setPhoneNumber(e.target.value)} />
        </div>
        
        <div>
          <label htmlFor="options">Options:</label>
          <select id="options" value={selectedOption} onChange={(e) => setSelectedOption(e.target.value)}>
            <option value="">Select an option</option>
            <option value="1">Employee</option>
            <option value="2">Element Chief</option>
            <option value="3">Flight Chief</option>
            <option value="4">Squadron Director</option>
          </select>
        </div>

        <button type="submit">Add Contact</button>
      </form>
    </div>
    
  );
}

export default AddContact;