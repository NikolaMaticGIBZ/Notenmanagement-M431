namespace Shared.DTOs;

/// <summary>
/// DTO for resending a 2FA request
/// </summary>
public class Resend2FARequest
{
    public int UserId { get; set; }
}