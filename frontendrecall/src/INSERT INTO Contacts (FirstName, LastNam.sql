

-- CREATE TABLE Logins (
--     Email varchar(255),
--     Password varchar(255),
--     );

-- Insert into Roster.dbo.roster_contacts(roster_id, contactID)
-- VALUES ('1', '4003');
-- Select * from Roster.dbo.roster_contacts

CREATE TABLE Roster_Contact (
    rosterId INT,
    contactId INT,
    PRIMARY KEY (rosterId, contactId),
    FOREIGN KEY (rosterId) REFERENCES Roster.dbo.rosters(rosterId),
    FOREIGN KEY (contactId) REFERENCES Roster.dbo.contacts(contactId)
);
