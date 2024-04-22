// Models/Contact.cs

namespace recall_roster.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


    public class Roster
    {
        [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int rosterId { get; set; }
        public required string name{ get; set; }

        public string description{get; set;}
      public ICollection<RosterContact>? RosterContacts { get; set; }
    }

