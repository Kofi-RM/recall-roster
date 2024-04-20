using System;

namespace recall_roster.Models
{
    public class Recall
    {
        public int recallId { get; set; }
        public int rosterId {get; set;}
        public string message { get; set; }
        public DateTime timeStarted { get; set; }

        
        public DateTime timeEnded { get; set; }
        public int active { get; set; }



        // Navigation properties
     
       
    }
}
