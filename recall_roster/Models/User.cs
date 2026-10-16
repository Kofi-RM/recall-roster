using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace recall_roster.Models {
  [Table("User")] // Specify the table name
public class User
{


public int Id { get; set; }
    public bool IsSystemAdministrator { get; set; }
    public int TokenVersion { get; set; }
    public bool IsEnabled { get; set; } = true;

    [Required]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    public string Email { get; set; } = "";

    [Required]
    public string Password { get; set; } = "";
}
}
