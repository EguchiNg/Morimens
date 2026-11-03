using System.ComponentModel.DataAnnotations;

namespace Morimens.Api.DTOs.Requests;

public class RegisterRequest
{
    [Required, EmailAddress, MaxLength(256)]
    public string Email    { get; set; } = "";

    [Required, MinLength(3), MaxLength(64)]
    public string Username { get; set; } = "";

    [Required, MinLength(8)]
    public string Password { get; set; } = "";

    [MaxLength(64)]
    public string? DisplayName { get; set; }
}

public class LoginRequest
{
    /// <summary>Email or username.</summary>
    [Required]
    public string Login    { get; set; } = "";

    [Required]
    public string Password { get; set; } = "";
}

public class RefreshRequest
{
    [Required]
    public string RefreshToken { get; set; } = "";
}
