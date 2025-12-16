using recall_roster.Data;
using recall_roster.Models;
using Microsoft.AspNetCore.Mvc;

    public class ResponseService : IResponseService
    {
        private readonly AppDbContext _context;

        private readonly IContactService _contactService;

        private readonly IRecallResultsService _recallResultsService;

        public ResponseService(AppDbContext context, IContactService contactService, IRecallResultsService recallResultsService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _contactService = contactService;
            _recallResultsService = recallResultsService;
        }

        public List<Response> GetAllResponses()
        {
            return _context.Responses.ToList();
        }

        public Response? GetResponse(int contactId, int recallId)
        {
            var response = _context.Responses.FirstOrDefault(r => r.contactId == contactId && r.recallId == recallId);
            return response;
        }

         public void AddResponse( string from, string body, int recallId) {
       
       Console.WriteLine("TOP OF ADD RESPONSE");
        var contact = _contactService.GetContactByNumber(from);
        if (contact == null) {
            throw new InvalidOperationException($"Contact with phone number {from} not found.");
        } else {
            Console.WriteLine("CONTACT IN ADD RESPONSE");
        }
    
    var recall = _recallResultsService.GetRecall(recallId);
        if (recall == null) {
            throw new InvalidOperationException("Incorrect recall Id");
        }
        
        var response = new Response
        {
            contactId = contact.contactId,
            response = body,
            responseTime = DateTime.UtcNow,
            recallId = recallId,
        };
        Console.WriteLine( $"contactId={response.contactId}, " +
    $"recallId={response.recallId}, " +
    $"response={response.response}, " +
    $"responseTime={response.responseTime}");
        try {
        _context.Responses.Add(response);
        } catch (Exception ex) {
            Console.WriteLine(ex.Message);
            
        }
        _context.SaveChanges();
        Console.WriteLine("ADDED RESPOMSE INSIDE ADD RESPONSE");

        
        // _logger.LogInformation("Response added successfully");
        // return CreatedAtAction(nameof(GetResponse), new { id = response.responseId }, response);
    
   


        }
    }

public interface IResponseService
{
    List<Response> GetAllResponses();
    Response? GetResponse(int contactId, int recallId);
    void AddResponse(string from, string body, int recallId);
}