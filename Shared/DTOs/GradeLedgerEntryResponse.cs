namespace Shared.DTOs;

public class GradeLedgerEntryResponse
{
    public int Id { get; set; }
    public int GradeId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string SnapshotJson { get; set; } = string.Empty;
    public string PayloadHash { get; set; } = string.Empty;
    public string PreviousHash { get; set; } = string.Empty;
    public string BlockHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int ActorUserId { get; set; }
    public string ActorRole { get; set; } = string.Empty;
}
