namespace Shared.DTOs;

/// <summary>
/// DTO for the response when veryfing a 2fa code
/// </summary>
public class Verify2FAResponse
{
    public string Token { get; set; } = string.Empty;
    public string Role { get; set; } = "teacher";
}