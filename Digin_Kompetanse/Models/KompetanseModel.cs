namespace Digin_Kompetanse.Models;

public class KompetanseModel
{
    public int KompetanseId { get; set; }
    public string Navn { get; set; }
    public string? Beskrivelse { get; set; }
    public BedriftModel? Bedrift { get; set; }
    public FagområdeModel? Fagområde { get; set; }
}