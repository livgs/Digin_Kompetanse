
namespace Digin_Kompetanse.Models;

public class Kompetanse
{
    public int KompetanseId { get; set; }

    public string? KompetanseKategori { get; set; }

    public virtual ICollection<UnderKompetanse> UnderKompetanses { get; set; } = new List<UnderKompetanse>();

    public virtual ICollection<Fagområde> Fagområdes { get; set; } = new List<Fagområde>();
}
