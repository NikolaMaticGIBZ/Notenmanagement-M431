using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.DataAccess.Models;

[Table("users")]
public class User
{
    [Key]
    public int Id { get; set; }

    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(255)]
    public string Password_hash { get; set; } = string.Empty;

    public ICollection<Grade> CreatedGrades { get; set; } = new List<Grade>();
    public ICollection<Grade> DecidedGrades { get; set; } = new List<Grade>();


    // 2FA
    public string? Twofactor_code { get; set; }
    public DateTime? Twofactor_expires { get; set; }
    public bool Twofactor_verified { get; set; }

    // Role
    public string Role { get; set; } = "teacher";
}
