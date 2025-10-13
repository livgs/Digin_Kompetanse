

namespace Digin_Kompetanse.Models;

public class Bedrift
{
    public int BedriftId { get; set; }

    public string BedriftNavn { get; set; } = null!;

    public string BedriftEpost { get; set; } = null!;

    public string? Beskrivelse { get; set; }

    public virtual ICollection<Fagområde> Fagområdes { get; set; } = new List<Fagområde>();
}

