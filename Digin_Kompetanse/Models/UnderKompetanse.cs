
namespace Digin_Kompetanse.Models;

public class UnderKompetanse
{
    public int UnderkompetanseId { get; set; }

    public string? UnderkompetanseNavn { get; set; }

    public int? KompetanseId { get; set; }

    public virtual Kompetanse? Kompetanse { get; set; }
}
