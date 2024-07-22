using System.ComponentModel.DataAnnotations;

namespace WebAPIUdemy.DTOs.AuthDTO;

public class RegisterModel
{
    [Required(ErrorMessage = "User name is required")]
    public string? UserName { get; set; }

    [EmailAddress]
    [Required(ErrorMessage = "Email is required")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Passoword is required")]
    public string? Password { get; set; }
}
