using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.DataAccess.Models;

[Table("users")]
public class Users
{
    [Key]
    public int id { get; set; }

    [MaxLength(50)]
    public string username { get; set; } = string.Empty;

    [EmailAddress]
    [MaxLength(255)]
    public string email { get; set; } = string.Empty;

    [MaxLength(255)]
    public string password_hash { get; set; } = string.Empty;

    public ICollection<Grades> Grades { get; set; } = new List<Grades>();

    // 2FA
    public string? twofactor_code { get; set; }
    public DateTime? twofactor_expires { get; set; }
    public bool twofactor_verified { get; set; }
}
