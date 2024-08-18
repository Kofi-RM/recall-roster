using System;
namespace recall_roster.Models
{
    public class Response
    {
        public int responseId { get; set; }
        public int contactId { get; set; }
        public int recallId { get; set; }
        public DateTime responseTime { get; set; }
        
        public string response {  get; set; }



        // Navigation properties
        public Contact Contact { get; set; }
        public Recall Recall { get; set; }
    }
}
