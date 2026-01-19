using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs;

/// <summary>
/// DTO for creating a Grade in UI as teacher
/// </summary>
public class GradeFormModel
{
    [Required(ErrorMessage = "Modul ist erforderlich.")]
    public string ModuleName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Klasse ist erforderlich.")]
    public string CourseName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Schülername ist erforderlich.")]
    public string StudentName { get; set; } = string.Empty;

    [Range(1, 6, ErrorMessage = "Die Note muss zwischen 1.0 und 6.0 liegen.")]
    public decimal GradeValue { get; set; } = 1m;

    [Required(ErrorMessage = "Bitte einen Rektor auswählen.")]
    public int RektorId { get; set; }
    public string Comment { get; set; } = string.Empty;
}