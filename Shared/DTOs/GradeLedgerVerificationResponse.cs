namespace Shared.DTOs;

public class GradeLedgerVerificationResponse
{
    public bool IsValid { get; set; }
    public string Message { get; set; } = string.Empty;
    public int? InvalidEntryId { get; set; }
    public string? LatestBlockHash { get; set; }
}
