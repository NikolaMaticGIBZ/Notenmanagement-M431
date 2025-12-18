using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.DataAccess.Models;

[Table("grades")]
public class Grades
{
    [Key]
    public int id { get; set; }

    [MaxLength(255)]
    public string course_name { get; set; } = string.Empty;

    [Range(1.0, 6.0)]
    public decimal grade_value { get; set; }

    [MaxLength(255)]
    public string student_name { get; set; } = string.Empty;

    public Users? teacher { get; set; }

    [ForeignKey(nameof(teacher))]
    public int teacher_id { get; set; }
    public Rektor? rektor { get; set; }

    [ForeignKey(nameof(rektor))]
    public int rektor_id { get; set; }
}
