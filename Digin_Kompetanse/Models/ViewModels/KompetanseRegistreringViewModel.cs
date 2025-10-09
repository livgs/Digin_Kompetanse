using System.ComponentModel.DataAnnotations;

namespace Digin_Kompetanse.Models.ViewModels
{
    public class KompetanseRegistreringViewModel
    {
        [Required] public string BedriftNavn { get; set; } = string.Empty;

        [Required, EmailAddress] public string BedriftEpost { get; set; } = string.Empty;

        [Required] public int FagområdeId { get; set; }
        
        [Required(ErrorMessage = "Velg minst én kompetanse")]
        public int? KompetanseId { get; set; } 
        
        public int? UnderkompetanseId { get; set; } 

        public string Beskrivelse { get; set; } = string.Empty;

    }

}