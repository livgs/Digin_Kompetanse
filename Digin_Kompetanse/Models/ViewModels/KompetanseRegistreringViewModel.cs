using System.ComponentModel.DataAnnotations;

namespace Digin_Kompetanse.Models.ViewModels
{
    public class KompetanseRegistreringViewModel
    {
        // Fjern kravene til disse (eller fjern dem helt fra modellen)
        public string? BedriftNavn { get; set; }
        public string? BedriftEpost { get; set; }

        [Required(ErrorMessage = "Velg et fagområde")]
        public int? FagområdeId { get; set; }  

        [Required(ErrorMessage = "Velg minst én kompetanse")]
        public int? KompetanseId { get; set; } 

        public int? UnderkompetanseId { get; set; }

        [StringLength(500, ErrorMessage = "Beskrivelse kan maks være 500 tegn")]
        public string? Beskrivelse { get; set; }
    }
}