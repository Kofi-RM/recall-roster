using recall_roster.Data;
using recall_roster.Models;

    public class RosterContactRepositoryService : IRosterContactRepository
    {
        private readonly AppDbContext _context;

        public RosterContactRepositoryService(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Method to retrieve all roster contacts
        public List<RosterContact> GetAllRosterContacts(int rosterId)
        {


            return _context.RosterContacts.Where(rc => rc.rosterId == rosterId).ToList();
        }


        // Method to retrieve roster contacts by roster ID
        public RosterContact? GetRosterContact(int rosterId, int contactId)
        {
            return _context.RosterContacts.FirstOrDefault(rc => rc.rosterId == rosterId && rc.contactId == contactId);
        }

        // Method to add roster contact
        public void AddRosterContact(RosterContact rosterContact)
        {
            _context.RosterContacts.Add(rosterContact);
            _context.SaveChanges();
        }

        // Method to remove roster contact
        public void RemoveRosterContact(RosterContact rosterContact)
        {
            _context.RosterContacts.Remove(rosterContact);
            _context.SaveChanges();
        }

        public void UpdateRosterContacts(int rosterId, int[] contactsToAdd, int[] contactsToRemove)
        {
            // Add new roster contacts
            foreach (var contact in contactsToAdd)
            {
                var rosterContactToAdd = new RosterContact
                {
                    rosterId = rosterId,
                    contactId = contact, // Assuming Contact class has an Id property
                };
                _context.RosterContacts.Add(rosterContactToAdd);
            }

            // Remove existing roster contacts
            foreach (var contact in contactsToRemove)
            {
                var rosterContactToRemove = _context.RosterContacts.FirstOrDefault(rc => rc.rosterId == rosterId && rc.contactId == contact);
                if (rosterContactToRemove != null)
                {
                    _context.RosterContacts.Remove(rosterContactToRemove);
                }
            }

            // Save changes to the database
            _context.SaveChanges();
        }
        // Add other methods for CRUD operations as needed...
    }

    public interface IRosterContactRepository
    {
        List<RosterContact> GetAllRosterContacts(int rosterId);
        RosterContact? GetRosterContact(int rosterId, int contactId);
        void AddRosterContact(RosterContact rosterContact);
        void RemoveRosterContact(RosterContact rosterContact);
        void UpdateRosterContacts(int rosterId, int[] contactsToAdd ,int[] contactsToRemove);



    }
