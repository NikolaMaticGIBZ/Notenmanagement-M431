using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.DataAccess.Models;

[Table("rektor")]
public class Rektor
{
    [Key]
    public int id { get; set; }

    [MaxLength(255)]
    public string name { get; set; } = string.Empty;

    public ICollection<Grades> Grades { get; set; } = new List<Grades>();
}
