using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.JavaScript;

namespace Digin_Kompetanse.Models;

public class FagområdeModel
{
    [Required(ErrorMessage = "Fagområde er nødvendig")]
    public string Navn { get; set; }
}