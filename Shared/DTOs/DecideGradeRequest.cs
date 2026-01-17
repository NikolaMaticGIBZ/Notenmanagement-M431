namespace Shared.DTOs;

public class DecideGradeRequest
{
    // "approved" or "rejected"
    public string Status { get; set; } = string.Empty;
    public string? DecisionNote { get; set; }
}
