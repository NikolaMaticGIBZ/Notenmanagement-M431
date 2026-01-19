namespace Shared.DTOs;

/// <summary>
/// DTO for the response of logging in
/// </summary>
public class LoginResponse
{
    public int UserId { get; set; }
    public bool RequiresTwoFactor { get; set; }
}