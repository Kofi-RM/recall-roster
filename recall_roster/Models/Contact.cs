// Models/Contact.cs

namespace recall_roster.Models
{
    public class Contact
    {
        public int contactId{ get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Rank {get; set;}

    public int UnitId { get; set; }
        public int Active {get; set; }

        // Validate every value before mutation so rejected edits leave the entity unchanged.
        public void UpdateDetails(string firstName, string lastName, string phoneNumber, string rank)
        {
            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || !StaffRanks.IsSupported(rank))
                throw new ArgumentException("First name, last name, and a supported staff rank are required.");
            var phone = phoneNumber?.Trim() ?? "";
            recall_roster.Services.PhoneNumbers.Normalize(phone);
            FirstName = firstName.Trim();
            LastName = lastName.Trim();
            PhoneNumber = phone;
            Rank = rank;
        }

        public void Deactivate() => Active = 0;

        [System.Text.Json.Serialization.JsonIgnore]
        public ICollection<RosterContact>? RosterContacts { get; set; }
    }
}
