using System.ComponentModel.DataAnnotations;

namespace Digin_Kompetanse.Models.ViewModels
{
    public class KompetanseRegistreringViewModel
    {
        // En innsending kan ha flere rader med kompetanse
        public List<KompetanseRadViewModel> Rader { get; set; } = new();
    }

    public class KompetanseRadViewModel
    {
        [Required(ErrorMessage = "Velg et fagområde")]
        public int? FagområdeId { get; set; }

        [Required(ErrorMessage = "Velg minst én kompetanse")]
        public int? KompetanseId { get; set; }
        
        public List<int> UnderkompetanseId { get; set; } = new();

        [StringLength(500, ErrorMessage = "Beskrivelse kan maks være 500 tegn")]
        public string? Beskrivelse { get; set; }
    }
}