namespace Shared.DTOs;

/// <summary>
/// DTO for the response of resending a 2fa code
/// </summary>
public class Resend2FAResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}