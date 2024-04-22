-- ALTER TABLE Roster.dbo.roster_contact
-- DROP COLUMN Role;

-- ALTER TABLE Roster.dbo.Contacts
-- ADD role INT; -- Adjust the size (50) as necessary

-- Select * from Roster.dbo.roster_contacts

-- EXEC sp_rename 'Roster.dbo.roster_contact.contact_id', 'contactId', 'COLUMN';

-- CREATE TABLE Roster_Contact (
--     rosterId INT,
--     contactId INT,
--     PRIMARY KEY (rosterId, contactId),
--     FOREIGN KEY (rosterId) REFERENCES Roster.dbo.Rosters(rosterId),
--     FOREIGN KEY (contactId) REFERENCES Roster.dbo.Contacts(contactId)
-- );

Select * from Roster.dbo.Logins