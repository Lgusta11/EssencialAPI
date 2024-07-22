using System.ComponentModel.DataAnnotations;

namespace WebAPIUdemy.DTOs.AuthDTO;

public class LoginModel
{
    [Required(ErrorMessage ="User name is required")]
    public string? UserName { get; set; }

    [Required(ErrorMessage = "Passoword is required")]
    public string? Password { get; set; }
}
