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

    public int UnitId { get; set; }
        public string description{get; set;} = "";
      [System.Text.Json.Serialization.JsonIgnore]
        public ICollection<RosterContact>? RosterContacts { get; set; }
    }

