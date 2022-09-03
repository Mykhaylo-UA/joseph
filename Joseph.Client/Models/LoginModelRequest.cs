using System.ComponentModel.DataAnnotations;
using ChristyAlsop.Wasm.Data.Attributes;

namespace Joseph.Client.Models;

public class LoginModelRequest
{
    [EmailAddress(ErrorMessage = "Email is not valid.")]
    [Required]
    public string Email { get; set; }

    [Required]
    [MinLength(6)]
    [RequiredDigit]
    public string Password { get; set; }
}