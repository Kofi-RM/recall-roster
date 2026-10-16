
using System.ComponentModel.DataAnnotations;

public class RegisterRequest
{
    [Required]
    [EmailAddress]
    [StringLength(320)]
    public string Email { get; set; } = "";

    [Required]
    [StringLength(128, MinimumLength = 12)]
    public string Password { get; set; } = "";
}
