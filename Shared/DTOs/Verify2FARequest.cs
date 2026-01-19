namespace Shared.DTOs;

/// <summary>
/// DTO for veryfing a 2FA request
/// </summary>
public class Verify2FARequest
{
    public int UserId { get; set; }
    public string Code { get; set; } = string.Empty;
}