
using System.ComponentModel.DataAnnotations;

public class RegisterRequest
{
    [Required]
    [EmailAddress]
    [StringLength(320)]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}
