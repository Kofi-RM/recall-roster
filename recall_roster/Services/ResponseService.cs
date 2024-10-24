using recall_roster.Data;
using recall_roster.Models;
using Microsoft.AspNetCore.Mvc;

    public class ResponseService : IResponseService
    {
        private readonly AppDbContext _context;

        private readonly IContactService _contactService;

        public ResponseService(AppDbContext context, IContactService contactService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _contactService = contactService;
        }

        public List<Response> GetAllResponses()
        {
            return _context.Responses.ToList();
        }

        public Response? GetResponse(int id)
        {
            var response = _context.Responses.FirstOrDefault(r => r.contactId == id);
            return response;
        }

         public string AddResponse( string from, string body, int recallId) {
       
       Console.WriteLine("TOP OF ADD RESPONSE");
        var contact = _contactService.GetContactByNumber(from);
        if (contact == null) {
            return $"Contact with phone number {from} not found.";
        } else {
            Console.WriteLine("CONTACT IN ADD RESPONSE");
        }
    // } catch (Exception ex) {
    //     _logger.LogError(ex, "Error adding response");
    //     return StatusCode(600, "Internal server error");
    // }
        
        var response = new Response
        {
            contactId = contact.contactID,
            response = body,
            responseTime = DateTime.UtcNow,
            recallId = recallId,
        };
        
        _context.Responses.Add(response);
        _context.SaveChanges();
        Console.WriteLine("ADDED RESPOMSE INSIDE ADD RESPONSE");

        return "Response added";
        // _logger.LogInformation("Response added successfully");
        // return CreatedAtAction(nameof(GetResponse), new { id = response.responseId }, response);
    
   


        }
    }

public interface IResponseService
{
    List<Response> GetAllResponses();
    Response? GetResponse(int id);
    string AddResponse(string from, string body, int recallId);
}