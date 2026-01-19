using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.DataAccess.Models;

[Table("grades")]
public class Grade
{
    [Key]
    public int Id { get; set; }

    [Column("course_name")]
    [MaxLength(255)]
    public string Course_name { get; set; } = string.Empty;

    [Column("module_name")]
    [MaxLength(255)]
    public string Module_name { get; set; } = string.Empty;

    [Column("grade_value")]
    [Range(1.0, 6.0)]
    public decimal Grade_value { get; set; }

    [Column("student_name")]
    [MaxLength(255)]
    public string Student_name { get; set; } = string.Empty;

    [Column("status")]
    [MaxLength(20)]
    public string Status { get; set; } = "pending";

    [Column("created_at")]
    public DateTime Created_at { get; set; } = DateTime.UtcNow;

    [Column("decision_note")]
    [MaxLength(1000)]
    public string? Decision_note { get; set; }

    public User? Teacher { get; set; }

    [Column("teacher_id")]
    [ForeignKey(nameof(Teacher))]
    public int Teacher_id { get; set; }

    public Rektor? Rektor { get; set; }

    [Column("rektor_id")]
    [ForeignKey(nameof(Rektor))]
    public int Rektor_id { get; set; }

    public User? Prorektor { get; set; }

    [Column("prorektor_id")]
    [ForeignKey(nameof(Prorektor))]
    public int? prorektor_id { get; set; }

    [Column("comment")]
    [MaxLength(1000)]
    public string? Comment { get; set; }
}
