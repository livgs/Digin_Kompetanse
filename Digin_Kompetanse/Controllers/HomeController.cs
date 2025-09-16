using Digin_Kompetanse.Models.ViewModels;
using Digin_Kompetanse.data;
using Digin_Kompetanse.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Digin_Kompetanse.Controllers;

public class HomeController : Controller
{
    private readonly KompetanseContext _context;

    public HomeController(KompetanseContext context)
    {
        _context = context;
    }

    [HttpGet]
public IActionResult Index()
{
    ViewBag.Fagområder = _context.Fagområde.AsNoTracking().ToList();
    return View(new KompetanseRegistreringViewModel());
}

    [HttpPost]
    public IActionResult Index(KompetanseRegistreringViewModel model)
{
    // Viktig: Sett alltid ViewBag før return
    ViewBag.Fagområder = _context.Fagområde.ToList();

    if (!ModelState.IsValid) return View(model);

    // Hent eller opprett bedrift
    var bedrift = _context.Bedrift
        .FirstOrDefault(b => b.BedriftNavn == model.BedriftNavn && b.BedriftEpost == model.BedriftEpost);

    if (bedrift == null)
    {
        bedrift = new Bedrift { BedriftNavn = model.BedriftNavn, BedriftEpost = model.BedriftEpost };
        _context.Bedrift.Add(bedrift);
        _context.SaveChanges();
    }

    // Fagområde
    var fagområde = _context.Fagområde.FirstOrDefault(f => f.FagområdeId == model.FagområdeId);
    if (fagområde == null) return NotFound("Fagområde ikke funnet");

    if (fagområde.BedriftId != bedrift.BedriftId)
    {
        fagområde.BedriftId = bedrift.BedriftId;
        _context.Fagområde.Update(fagområde);
        _context.SaveChanges();
    }
    
    if (model.KompetanseId.Any())
    {
        foreach (var kId in model.KompetanseId)
        {
            var kompetanse = _context.Kompetanse
                .Include(k => k.Fagområdes)
                .Include(k => k.UnderKompetanses)
                .FirstOrDefault(k => k.KompetanseId == kId);

            if (kompetanse == null) continue;

            // Legg til fagområde hvis det ikke allerede er lagt til
            if (!kompetanse.Fagområdes.Any(f => f.FagområdeId == fagområde.FagområdeId))
                kompetanse.Fagområdes.Add(fagområde);

            // Underkompetanse (valgfritt)
            if (model.UnderkompetanseId.HasValue)
            {
                var underkompetanse = _context.UnderKompetanse
                    .FirstOrDefault(uk => uk.UnderkompetanseId == model.UnderkompetanseId.Value);

                if (underkompetanse != null && underkompetanse.KompetanseId == kompetanse.KompetanseId)
                {
                    if (!kompetanse.UnderKompetanses.Any(uk => uk.UnderkompetanseId == underkompetanse.UnderkompetanseId))
                        kompetanse.UnderKompetanses.Add(underkompetanse);
                }
            }

            _context.Kompetanse.Update(kompetanse);
        }

        _context.SaveChanges();
    }

    return RedirectToAction("Overview");
}
    [HttpGet]
    public JsonResult GetKompetanser(int fagområdeId)
    {
        var kompetanser = _context.Kompetanse
            .Where(k => k.Fagområdes.Any(f => f.FagområdeId == fagområdeId))
            .Select(k => new { k.KompetanseId, k.KompetanseKategori })
            .ToList();

        return Json(kompetanser);
    }

    [HttpGet]
    public JsonResult GetUnderkompetanser(int kompetanseId)
    {
        var underkompetanser = _context.UnderKompetanse
            .Where(uk => uk.KompetanseId == kompetanseId)
            .Select(uk => new { uk.UnderkompetanseId, uk.UnderkompetanseNavn })
            .ToList();

        return Json(underkompetanser);
    }

    [HttpGet]
    public IActionResult Overview()
    {
        var kompetanser = _context.Kompetanse
            .Include(k => k.UnderKompetanses)
            .Include(k => k.Fagområdes)
                .ThenInclude(f => f.Bedrift)
            .ToList();

        return View(kompetanser);
    }
    
    public IActionResult Admin()
    {
        var viewModel = Queryable.SelectMany(
                _context.Fagområde
                    .Where(f => f.Bedrift != null),
                f => f.Kompetanses.Select(k => new AdminViewModel
                {
                    BedriftId = f.Bedrift!.BedriftId,
                    BedriftNavn = f.Bedrift!.BedriftNavn,
                    Epost = f.Bedrift!.BedriftEpost,
                    Fagområde = f.FagområdeNavn!,
                    KompetanseKategori = k.KompetanseKategori!
                }))
            .ToList();



        return View(viewModel);
    }



    public IActionResult Privacy() => View();
    public IActionResult Help() => View();
    

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
