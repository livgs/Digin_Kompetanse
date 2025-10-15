using Digin_Kompetanse.Models.ViewModels;
using Digin_Kompetanse.data;
using Digin_Kompetanse.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;

namespace Digin_Kompetanse.Controllers
{
    public class HomeController : Controller
    {
        private readonly KompetanseContext _context;

        // ✅ Constructor for dependency injection
        public HomeController(KompetanseContext context)
        {
            _context = context;
        }

        // Hjelper: bygg SelectList for fagområder
        private SelectList BuildFagomradeSelectList(int? selectedId = null)
        {
            var items = _context.Fagområde
                .AsNoTracking()
                .OrderBy(f => f.FagområdeNavn)
                .Select(f => new { f.FagområdeId, f.FagområdeNavn })
                .ToList();

            return new SelectList(items, "FagområdeId", "FagområdeNavn", selectedId);
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Fagområder = BuildFagomradeSelectList();
            return View(new KompetanseRegistreringViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(KompetanseRegistreringViewModel model)
        {
            // Repopulate dropdowns ved valideringsfeil
            ViewBag.Fagområder = BuildFagomradeSelectList(model.FagområdeId);

            if (!ModelState.IsValid) return View(model);

            // 1) Finn/lag bedrift (navn + epost)
            var bedrift = _context.Bedrift
                .FirstOrDefault(b => b.BedriftNavn == model.BedriftNavn && b.BedriftEpost == model.BedriftEpost);

            if (bedrift == null)
            {
                bedrift = new Bedrift
                {
                    BedriftNavn = model.BedriftNavn,
                    BedriftEpost = model.BedriftEpost
                };
                _context.Bedrift.Add(bedrift);
                _context.SaveChanges();
            }

            // 2) Sjekk at fagområde/kompetanse finnes
            var fagområde = _context.Fagområde
                .AsNoTracking()
                .FirstOrDefault(f => f.FagområdeId == model.FagområdeId);
            if (fagområde == null) return NotFound("Fagområde ikke funnet.");

            if (!model.KompetanseId.HasValue) return BadRequest("Kompetanse må velges.");

            var kompetanse = _context.Kompetanse
                .AsNoTracking()
                .FirstOrDefault(k => k.KompetanseId == model.KompetanseId.Value);
            if (kompetanse == null) return NotFound("Kompetanse ikke funnet.");

            // 3) Valgfri underkompetanse-validering
            int? underId = null;
            if (model.UnderkompetanseId.HasValue)
            {
                var under = _context.UnderKompetanse
                    .AsNoTracking()
                    .FirstOrDefault(uk => uk.UnderkompetanseId == model.UnderkompetanseId.Value
                                          && uk.KompetanseId == kompetanse.KompetanseId);
                if (under == null) return BadRequest("Ugyldig underkompetanse for valgt kompetanse.");
                underId = under.UnderkompetanseId;
            }

            // 4) Lag/oppdater rad i bedrift_kompetanse
            var eksisterende = _context.BedriftKompetanse.FirstOrDefault(bk =>
                bk.BedriftId == bedrift.BedriftId &&
                bk.FagområdeId == fagområde.FagområdeId &&
                bk.KompetanseId == kompetanse.KompetanseId &&
                bk.UnderKompetanseId == underId
            );

            if (eksisterende == null)
            {
                var bk = new BedriftKompetanse
                {
                    BedriftId = bedrift.BedriftId,
                    FagområdeId = fagområde.FagområdeId,
                    KompetanseId = kompetanse.KompetanseId,
                    UnderKompetanseId = underId,
                    Beskrivelse = model.Beskrivelse,
                    CreatedAt = DateTime.UtcNow
                };

                _context.BedriftKompetanse.Add(bk);
            }
            else
            {
                eksisterende.Beskrivelse = model.Beskrivelse;
            }

            _context.SaveChanges();
            return RedirectToAction("Overview");
        }

        [HttpGet]
        public JsonResult GetKompetanser(int fagområdeId)
        {
            var kompetanser = _context.Fagområde
                .AsNoTracking()
                .Include(f => f.Kompetanses)
                .Where(f => f.FagområdeId == fagområdeId)
                .SelectMany(f => f.Kompetanses)
                .OrderBy(k => k.KompetanseKategori)
                .Select(k => new { k.KompetanseId, k.KompetanseKategori })
                .ToList();

            return Json(kompetanser);
        }

        [HttpGet]
        public JsonResult GetUnderkompetanser(int kompetanseId)
        {
            var underkompetanser = _context.UnderKompetanse
                .AsNoTracking()
                .Where(uk => uk.KompetanseId == kompetanseId)
                .OrderBy(uk => uk.UnderkompetanseNavn)
                .Select(uk => new { uk.UnderkompetanseId, uk.UnderkompetanseNavn })
                .ToList();

            return Json(underkompetanser);
        }
        
    [HttpGet]
    public IActionResult Overview()
    {
        var role = HttpContext.Session.GetString("Role");
        var bedriftId = HttpContext.Session.GetInt32("BedriftId");
        
        if (role != "Bedrift" || bedriftId is null)
            return RedirectToAction("Index", "Home"); // evt. return Unauthorized();
        
        var rader = _context.BedriftKompetanse
            .Where(bk => bk.BedriftId == bedriftId.Value)
            .Include(bk => bk.Bedrift)
            .Include(bk => bk.Fagområde)
            .Include(bk => bk.Kompetanse)
            .Include(bk => bk.UnderKompetanse)
            .AsNoTracking()
            .OrderBy(bk => bk.Bedrift.BedriftNavn)
            .ThenBy(bk => bk.Fagområde.FagområdeNavn)
            .ThenBy(bk => bk.Kompetanse.KompetanseKategori)
            .ToList();

        return View(rader);
    }

        public IActionResult Privacy() => View();
        public IActionResult Help() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        
        [HttpGet]
        public IActionResult UserLogin()
        {
            // Hvis brukeren allerede er logget inn, send rett til forsiden
            var username = HttpContext.Session.GetString("Username");
            if (!string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public IActionResult UserLogin(string username, string password)
        {
            // Foreløpig "fake login" – du legger inn engangskode senere
            // Her kan du evt. validere brukeren mot databasen
            if (username == "TestBedrift" && password == "1234")
            {
                HttpContext.Session.SetString("Username", username);
                HttpContext.Session.SetString("Role", "Bedrift");
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Feil brukernavn eller passord.";
            return View();
        }

    }
}
