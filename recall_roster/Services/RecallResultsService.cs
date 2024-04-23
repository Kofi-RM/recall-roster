using recall_roster.Data;
using recall_roster.Models;



    public class RecallResultsService : IRecallResultsService
    {
        private readonly AppDbContext _context;

        public RecallResultsService(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public List<Recall> GetAllRecalls()
        {
            return _context.Recalls.ToList();
        }

        public Recall? GetRecall(int id)
        {
            var recall = _context.Recalls.FirstOrDefault(r => r.recallId == id);
            return recall;
        }

        public void AddRecall(Recall recall)
        {
            _context.Recalls.Add(recall);
            _context.SaveChanges();

        }
    }

public interface IRecallResultsService
{
    List<Recall> GetAllRecalls();
    Recall? GetRecall(int id);
    void AddRecall(Recall recall);
}