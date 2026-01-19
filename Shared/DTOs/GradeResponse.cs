namespace Shared.DTOs;

/// <summary>
/// DTO for the response of a grade
/// </summary>
public class GradeResponse
{
    public int Id { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public decimal GradeValue { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string ModuleName { get; set; } = string.Empty;

    public int RektorId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public string RektorName { get; set; } = string.Empty;

    public string Status { get; set; } = "pending";
    public DateTime CreatedAt { get; set; }
    public string? DecisionNote { get; set; }
    public string? Comment { get; set; }
}