using Digin_Kompetanse.data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Digin_Kompetanse.Controllers;

[Route("underkompetanse")]
public class UnderkompetanseController : Controller
{
    private readonly KompetanseContext _context;

    public UnderkompetanseController(KompetanseContext context)
    {
        _context = context;
    }
    [HttpGet("byKompetanse")]
    public async Task<IActionResult> GetByKompetanse(string kompetanse)
    {
        if (string.IsNullOrWhiteSpace(kompetanse))
            return Json(new List<string>());

        var underkompetanser = await _context.UnderKompetanse
            .Where(u => u.Kompetanse.KompetanseKategori == kompetanse)
            .Select(u => u.UnderkompetanseNavn)
            .Distinct()
            .ToListAsync();

        return Json(underkompetanser);
    }
}