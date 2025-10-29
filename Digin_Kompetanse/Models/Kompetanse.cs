
namespace Digin_Kompetanse.Models;

public class Kompetanse
{
    public int KompetanseId { get; set; }

    public string? KompetanseKategori { get; set; }

    public virtual ICollection<UnderKompetanse> UnderKompetanser { get; set; } = new List<UnderKompetanse>();

    public virtual ICollection<Fagområde> Fagområder { get; set; } = new List<Fagområde>();
}
