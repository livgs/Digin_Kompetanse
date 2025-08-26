namespace Digin_Kompetanse.Models;

public class KompetanseModel
{
    public string KompetanseNavn { get; set; }
    public BedriftModel Bedrift { get; set; }
    public FagområdeModel Fagområde { get; set; }
    public string Beskrivelse { get; set; }
}