using System.ComponentModel.DataAnnotations.Schema;

namespace recall_roster.Models
{

    [Table("Logins")] // Specify the table name
    public class LoginModel
    {
        public int Id { get; set; } // Example primary key property
        public required string Email { get; set; }
    public required string Password { get; set; }
    }
    }