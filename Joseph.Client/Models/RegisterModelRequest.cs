using System.ComponentModel.DataAnnotations;
using ChristyAlsop.Wasm.Data.Attributes;

namespace Joseph.Client.Models;

public class RegisterModelRequest
{
    [EmailAddress(ErrorMessage = "Email is not valid.")]
    [Required]
    public string Email { get; set; }

    [Required]
    [MinLength(6)]
    [RequiredDigit]
    public string Password { get; set; }

    public string Role { get; set; } = "Candidate";
}