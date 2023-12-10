// Models/Contact.cs

namespace recall_roster.Models
{
    public class Contact
    {
        public int ContactID { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string PhoneNumber { get; set; }
    }
}
