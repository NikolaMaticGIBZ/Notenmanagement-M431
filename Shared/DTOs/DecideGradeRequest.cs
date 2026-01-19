namespace Shared.DTOs;

/// <summary>
/// DTO for deciding grade requets // pending => approved
/// </summary>
public class DecideGradeRequest
{
    public string Status { get; set; } = string.Empty;
    public string? DecisionNote { get; set; }
}