namespace recall_roster.DTOs
{
    public class ContactCreateDto
    {
           public int contactID{ get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Rank {get; set;}

        public int Active {get; set; }
    }
}

// Additional Model specifically for adding Contacts.
// Need to avoid the optional RosterContact that it looks for and only
// add the Contact details