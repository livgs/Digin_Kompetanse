namespace Digin_Kompetanse.Models;

public class Fagområde
{
    public int FagområdeId { get; set; }

    public string? FagområdeNavn { get; set; }

    public virtual ICollection<Kompetanse> Kompetanses { get; set; } = new List<Kompetanse>();
}


