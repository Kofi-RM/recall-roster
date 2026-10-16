using recall_roster.Models;
using recall_roster.DTOs;

public interface IContactService
{
    Contact? GetContactById(int contactId);
    Contact? GetContactByNumber(string number);
    List<Contact> GetAllContacts();
    void RemoveContact(Contact contact);
    Contact AddContact(ContactCreateDto contact);
    Contact UpdateContact(Contact contact);
}
