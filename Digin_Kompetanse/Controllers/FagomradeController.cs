using Microsoft.AspNetCore.Mvc;
using Digin_Kompetanse.Models;
using Microsoft.EntityFrameworkCore;

namespace Digin_Kompetanse.Controllers;

public class FagomradeController : Controller
{
    private readonly AppDbContext _context;

    public FagomradeController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var fagomrader = await _context.Fagområde.ToListAsync();
        return View(fagomrader);
    }
}