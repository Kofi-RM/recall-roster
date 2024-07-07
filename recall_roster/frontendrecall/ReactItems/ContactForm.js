import React, { useState } from 'react';
import axios from 'axios'; // Import Axios for making HTTP requests


function ContactForm() {
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [phoneNumber, setPhoneNumber] = useState('');

  const handleSubmit = (event) => {
    event.preventDefault();

    // Make a POST request to the backend API to add a new contact
    axios.post('/api/contact', {
      firstName,
      lastName,
      phoneNumber
    })
    .then(response => {
      console.log('Contact added successfully:', response.data);
      // Clear form fields after successful submission
      setFirstName('');
      setLastName('');
      setPhoneNumber('');
    })
    .catch(error => {
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
        <button type="submit">Add Contact</button>
      </form>
    </div>
  );
}

export default ContactForm;