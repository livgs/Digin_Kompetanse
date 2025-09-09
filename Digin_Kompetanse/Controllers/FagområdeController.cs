using Digin_Kompetanse.data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Digin_Kompetanse.Controllers;

public class FagområdeController : Controller
{
    private readonly KompetanseContext _context;

    public FagområdeController(KompetanseContext context)
    {
        _context = context;
    }

    
    [HttpGet]
    public async Task<IActionResult> GetFagområde()
    {
        var fagomrader = await _context.Fagområde
            .Include(f => f.Kompetanses)
            .ToListAsync();
        return Json(fagomrader);
    }
}