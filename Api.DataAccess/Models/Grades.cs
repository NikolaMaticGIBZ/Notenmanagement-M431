using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.DataAccess.Models;

[Table("grades")]
public class Grades
{
    [Key]
    public int id { get; set; }

    [Column("course_name")]
    [MaxLength(255)]
    public string course_name { get; set; } = string.Empty;

    [Column("module_name")]
    [MaxLength(255)]
    public string module_name { get; set; } = string.Empty;

    [Column("grade_value")]
    [Range(1.0, 6.0)]
    public decimal grade_value { get; set; }

    [Column("student_name")]
    [MaxLength(255)]
    public string student_name { get; set; } = string.Empty;

    [Column("status")]
    [MaxLength(20)]
    public string status { get; set; } = "pending";

    [Column("created_at")]
    public DateTime created_at { get; set; } = DateTime.UtcNow;

    [Column("decision_note")]
    [MaxLength(1000)]
    public string? decision_note { get; set; }

    public Users? teacher { get; set; }

    [Column("teacher_id")]
    [ForeignKey(nameof(teacher))]
    public int teacher_id { get; set; }

    public Rektor? rektor { get; set; }

    [Column("rektor_id")]
    [ForeignKey(nameof(rektor))]
    public int rektor_id { get; set; }

    public Users? prorektor { get; set; }

    [Column("prorektor_id")]
    [ForeignKey(nameof(prorektor))]
    public int? prorektor_id { get; set; }
}
