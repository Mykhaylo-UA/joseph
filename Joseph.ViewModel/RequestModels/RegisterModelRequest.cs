using System.ComponentModel.DataAnnotations;

namespace Joseph.WebApi.RequestModels;

public class RegisterModelRequest
{
    [EmailAddress]
    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; }
    
    public string Role { get; set; }
}