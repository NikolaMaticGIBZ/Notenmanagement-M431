namespace Shared.DTOs;

/// <summary>
/// DTO for updating a grade request
/// </summary>
public class UpdateGradeRequest
{
    public string CourseName { get; set; } = string.Empty;
    public string ModuleName { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public decimal GradeValue { get; set; }
    public int RektorId { get; set; }
    public string? Comment { get; set; }
}