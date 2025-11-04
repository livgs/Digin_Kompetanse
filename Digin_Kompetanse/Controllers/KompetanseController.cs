using Digin_Kompetanse.data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Digin_Kompetanse.Controllers;

[Route("kompetanse")]
public class KompetanseController : Controller
{
    private readonly KompetanseContext _context;

    public KompetanseController(KompetanseContext context)
    {
        _context = context;
    }
    [HttpGet("byFagomrade")]
    public async Task<IActionResult> GetByFagomrade(string fagomrade)
    {
        if (string.IsNullOrWhiteSpace(fagomrade))
            return Json(new List<string>());

        var kompetanser = await _context.Kompetanse
            .Where(k => k.Fagområder.Any(f => f.FagområdeNavn == fagomrade))
            .Select(k => k.KompetanseKategori)
            .Distinct()
            .ToListAsync();

        return Json(kompetanser);
    }
}