namespace Shared.DTOs;

/// <summary>
/// DTO for creating a grade request
/// </summary>
public class CreateGradeRequest
{
    public string CourseName { get; set; } = string.Empty;
    public decimal GradeValue { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string ModuleName { get; set; } = string.Empty;
    public int RektorId { get; set; }
    public string? Comment { get; set; }
}