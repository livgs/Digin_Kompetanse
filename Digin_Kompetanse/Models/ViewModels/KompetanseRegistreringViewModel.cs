using System.ComponentModel.DataAnnotations;

namespace Digin_Kompetanse.Models.ViewModels
{
    public class KompetanseRegistreringViewModel
    {
        [Required(ErrorMessage = "Bedriftens navn må fylles ut")]
        [StringLength(100, ErrorMessage = "Navnet kan maks være 100 tegn")]
        public string BedriftNavn { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "E-post er påkrevd")]
        [EmailAddress(ErrorMessage = "Skriv inn en gyldig e-postadresse, f.eks: navn@firma.no")]
        [StringLength(100)]
        public string BedriftEpost { get; set; } = string.Empty;

        [Required(ErrorMessage = "Velg et fagområde")]
        public int FagområdeId { get; set; }
        
        [Required(ErrorMessage = "Velg minst én kompetanse")]
        public int? KompetanseId { get; set; } 
        
        public int? UnderkompetanseId { get; set; } 

        [StringLength(500, ErrorMessage = "Beskrivelse kan maks være 500 tegn")]
        public string? Beskrivelse { get; set; } 

    }

}