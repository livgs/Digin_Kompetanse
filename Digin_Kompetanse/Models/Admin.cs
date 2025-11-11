using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Digin_Kompetanse.Models;

[Table("admin")]
public class Admin
{
    [Key]
    [Column("admin_id")]
    public int AdminId { get; set; }

    [Column("admin_epost")]
    public string AdminEpost { get; set; } = null!;

    [Column("admin_passord_hash")]
    public string AdminPassordHash { get; set; } = null!;

    [Column("navn")]
    public string? Navn { get; set; }
    
    [Column("failed_attempts")]
    public int FailedAttempts { get; set; }

    [Column("lockout_until")]
    public DateTime? LockoutUntil { get; set; }
}

