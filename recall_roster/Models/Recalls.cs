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

              public int Employees { get; set; }
     public int ElementChief { get; set;}
     public int FlightChief { get; set;}

     public int SquadronDirector { get; set;}
     public int Total { get; set;}
     public int EmployeesMax { get; set;}
     public int ElementChiefMax { get; set;}
     public int FlightChiefMax { get; set;}
     public int SquadronDirectorMax { get; set;}

          public int TotalMax { get; set;}

    
        public int active { get; set; }



        // Navigation properties
     
       
    }
}
