using System.ComponentModel.DataAnnotations;
using ChristyAlsop.Wasm.Data.Attributes;

namespace ChristyAlsop.Wasm.Data;

public class AuthRequest
{
    [EmailAddress(ErrorMessage = "Email is not valid.")]
    [Required]
    public string Email { get; set; }
    
    [Required]
    [MinLength(6)]
    [RequiredDigit]
    public string Password { get; set; }
}