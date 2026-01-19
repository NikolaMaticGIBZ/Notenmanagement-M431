namespace Shared.DTOs;

/// <summary>
/// DTO for a login request
/// </summary>
public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}