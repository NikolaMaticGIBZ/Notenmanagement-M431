using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.DataAccess.Models;

[Table("grade_ledger")]
public class GradeLedgerEntry
{
    [Key]
    public int Id { get; set; }

    [Column("grade_id")]
    public int Grade_id { get; set; }

    [Column("action")]
    [MaxLength(50)]
    public string Action { get; set; } = string.Empty;

    [Column("snapshot_json")]
    [MaxLength(4000)]
    public string Snapshot_json { get; set; } = string.Empty;

    [Column("payload_hash")]
    [MaxLength(128)]
    public string Payload_hash { get; set; } = string.Empty;

    [Column("previous_hash")]
    [MaxLength(128)]
    public string Previous_hash { get; set; } = string.Empty;

    [Column("block_hash")]
    [MaxLength(128)]
    public string Block_hash { get; set; } = string.Empty;

    [Column("created_at")]
    public DateTime Created_at { get; set; } = DateTime.UtcNow;

    [Column("actor_user_id")]
    public int Actor_user_id { get; set; }

    [Column("actor_role")]
    [MaxLength(50)]
    public string Actor_role { get; set; } = string.Empty;
}
