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
        ViewBag.Fagområder = _context.Fagområde
            .AsNoTracking()
            .Include(f => f.Kompetanses)
            .ToList();
        return View(new KompetanseRegistreringViewModel());
    }

 [HttpPost]
public IActionResult Index(KompetanseRegistreringViewModel model)
{
    // Sett ViewBag for visning hvis ModelState ikke er valid
    ViewBag.Fagområder = _context.Fagområde
        .Include(f => f.Kompetanses)
        .ToList();

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

    // Hent fagområde
    var fagområde = _context.Fagområde.FirstOrDefault(f => f.FagområdeId == model.FagområdeId);
    if (fagområde == null) return NotFound("Fagområde ikke funnet");

    // Koble fagområde til bedrift
    if (fagområde.BedriftId != bedrift.BedriftId)
    {
        fagområde.BedriftId = bedrift.BedriftId;
        _context.Fagområde.Update(fagområde);
        _context.SaveChanges();
    }

    // Håndter valgt kompetanse
    if (model.KompetanseId.HasValue)
    {
        // Finn kompetanse
        var kompetanse = _context.Kompetanse
            .Include(k => k.Fagområdes)
            .Include(k => k.UnderKompetanses)
            .FirstOrDefault(k => k.KompetanseId == model.KompetanseId.Value);

        if (kompetanse != null)
        {
            // Fjern alle eksisterende koblinger til dette fagområdet først
            var eksisterendeKobling = kompetanse.Fagområdes.FirstOrDefault(f => f.FagområdeId == fagområde.FagområdeId);
            if (eksisterendeKobling != null)
            {
                kompetanse.Fagområdes.Remove(eksisterendeKobling);
            }

            // Legg til kun den valgte kompetansen
            kompetanse.Fagområdes.Add(fagområde);

            // Håndter valgt underkompetanse
            if (model.UnderkompetanseId.HasValue)
            {
                // Fjern eventuelle eksisterende underkompetanser for denne kompetansen
                kompetanse.UnderKompetanses.Clear();

                var underkompetanse = _context.UnderKompetanse
                    .FirstOrDefault(uk => uk.UnderkompetanseId == model.UnderkompetanseId.Value &&
                                          uk.KompetanseId == kompetanse.KompetanseId);

                if (underkompetanse != null)
                {
                    kompetanse.UnderKompetanses.Add(underkompetanse);
                }
            }

            _context.Kompetanse.Update(kompetanse);
            _context.SaveChanges();
        }
    }

    return RedirectToAction("Overview");
}

    [HttpGet]
    public JsonResult GetKompetanser(int fagområdeId)
    {
        var fagområde = _context.Fagområde
            .Include(f => f.Kompetanses)
            .FirstOrDefault(f => f.FagområdeId == fagområdeId);

        if (fagområde == null) return Json(new List<object>());

        var kompetanser = fagområde.Kompetanses
            .Select(k => new { k.KompetanseId, k.KompetanseKategori })
            .ToList();

        return Json(kompetanser, new System.Text.Json.JsonSerializerOptions
        {
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
        });
    }

    [HttpGet]
    public JsonResult GetUnderkompetanser(int kompetanseId)
    {
        var underkompetanser = _context.UnderKompetanse
            .Where(uk => uk.KompetanseId == kompetanseId)
            .Select(uk => new { uk.UnderkompetanseId, uk.UnderkompetanseNavn })
            .ToList();

        return Json(underkompetanser, new System.Text.Json.JsonSerializerOptions
        {
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
        });
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
