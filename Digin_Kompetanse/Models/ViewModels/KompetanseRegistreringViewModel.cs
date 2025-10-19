using System.ComponentModel.DataAnnotations;

namespace Digin_Kompetanse.Models.ViewModels
{
    public class KompetanseRegistreringViewModel
    {
        [Required(ErrorMessage = "Bedriftens navn må fylles ut")]
        public string BedriftNavn { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "E-post er påkrevd")]
        [EmailAddress(ErrorMessage = "Skriv inn en gyldig e-postadresse, f.eks: navn@firma.no")]
        public string BedriftEpost { get; set; } = string.Empty;

        [Required(ErrorMessage = "Velg et fagområde")]
        public int FagområdeId { get; set; }
        
        [Required(ErrorMessage = "Velg minst én kompetanse")]
        public int? KompetanseId { get; set; } 
        
        public int? UnderkompetanseId { get; set; } 

        public string Beskrivelse { get; set; } = string.Empty;

    }

}