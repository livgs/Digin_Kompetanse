namespace Digin_Kompetanse.Models
{
    public class BedriftKompetanse
    {
        public int Id { get; set; }

        public int BedriftId { get; set; }
        public int FagområdeId { get; set; }         
        public int KompetanseId { get; set; }
        public int? UnderKompetanseId { get; set; }  

        public string? Beskrivelse { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public bool IsActive { get; set; } = true;
        public DateTime? ModifiedAt { get; set; }
        public int? ModifiedByBedriftId { get; set; }

        public Bedrift Bedrift { get; set; } = null!;
        public Fagområde Fagområde { get; set; } = null!;
        public Kompetanse Kompetanse { get; set; } = null!;
        public UnderKompetanse? UnderKompetanse { get; set; }
    }
}