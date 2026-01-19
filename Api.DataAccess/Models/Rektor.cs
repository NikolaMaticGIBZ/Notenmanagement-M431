using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.DataAccess.Models;

[Table("rektor")]
public class Rektor
{
    [Key]
    public int Id { get; set; }

    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    public ICollection<Grade> Grades { get; set; } = new List<Grade>();
}
