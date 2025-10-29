namespace Digin_Kompetanse.Models.ViewModels
{
    public class AdminViewModel
    {
        public int BedriftId { get; set; }
        public string BedriftNavn { get; set; } = string.Empty;
        public string? Epost { get; set; }
        public string Fagområde { get; set; } = string.Empty;
        public string KompetanseKategori { get; set; } = string.Empty;
        public string? UnderKompetanse { get; set; }   // ✅ Capital K
        public string? Beskrivelse { get; set; }
    }
}