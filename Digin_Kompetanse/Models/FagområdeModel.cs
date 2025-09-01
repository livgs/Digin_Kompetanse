using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.JavaScript;

namespace Digin_Kompetanse.Models;

public class FagområdeModel
{
    [Required(ErrorMessage = "Fagområde er nødvendig")]
    public int FagområdeId { get; set; }
    public string Navn { get; set; }
    public List<KompetanseModel>? Kompetanser { get; set; }
}