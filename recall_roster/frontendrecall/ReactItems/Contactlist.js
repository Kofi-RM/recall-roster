import React, { useState, useEffect } from 'react';
import axios from 'axios'; // Import Axios for making HTTP requests

function ContactList() {
  const [contacts, setContacts] = useState([]);

  useEffect(() => {
    // Fetch contacts from the backend API when the component mounts
    axios.get('/api/contact')
      .then(response => {
        setContacts(response.data);
      })
      .catch(error => {
        console.error('Error fetching contacts:', error);
      });
  }, []); // Empty dependency array ensures the effect runs only once when the component mounts

  return (
    <div>
      <h2>Contact List</h2>
      <ul>
        {contacts.map(contact => (
          <li key={contact.contactID}>
            {contact.firstName} {contact.lastName} - {contact.phoneNumber}
          </li>
        ))}
      </ul>
    </div>
  );
}
export default ContactList;
