using recall_roster.Data;
using recall_roster.Models;

    public class RecallResponseService : IResponseService
    {
        private readonly AppDbContext _context;

        public RecallResponseService(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public List<Response> GetAllResponses()
        {
            return _context.Responses.ToList();
        }

        public Response? GetResponse(int id)
        {
            var response = _context.Responses.FirstOrDefault(r => r.responseId == id);
            return response;
        }

         public void AddResponse(Response response)
        {
            _context.Responses.Add(response);
            _context.SaveChanges();
        }
    }

public interface IResponseService
{
    List<Response> GetAllResponses();
    Response? GetResponse(int id);
    void AddResponse(Response response);
}