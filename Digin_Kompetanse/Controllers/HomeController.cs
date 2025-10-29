using Digin_Kompetanse.Models.ViewModels;
using Digin_Kompetanse.data;
using Digin_Kompetanse.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Digin_Kompetanse.Controllers
{
    public class HomeController : Controller
    {
        private readonly KompetanseContext _context;

        public HomeController(KompetanseContext context)
        {
            _context = context;
        }
        
        private SelectList BuildFagomradeSelectList(int? selectedId = null)
        {
            var items = _context.Fagområde
                .AsNoTracking()
                .OrderBy(f => f.FagområdeNavn)
                .Select(f => new { f.FagområdeId, f.FagområdeNavn })
                .ToList();

            return new SelectList(items, "FagområdeId", "FagområdeNavn", selectedId);
        }

        // Viser innloggingssiden (Views/Auth/Login.cshtml)
        [HttpGet("/auth/login")]
        public IActionResult Login()
        {
            // Hvis allerede innlogget som bedrift → gå til oversikt
            if (HttpContext.Session.GetString("Role") == "Bedrift" &&
                HttpContext.Session.GetInt32("BedriftId") is not null)
            {
                return RedirectToAction(nameof(Overview));
            }
            
            return View("~/Views/Auth/Login.cshtml");
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var row = _context.BedriftKompetanse.Find(id);
            if (row == null)
            {
                TempData["Error"] = "Fant ikke posten.";
                return RedirectToAction(nameof(Overview));
            }

            _context.BedriftKompetanse.Remove(row); // eller row.IsDeleted = true;
            _context.SaveChanges();

            TempData["Success"] = "Kompetansen ble slettet.";
            return RedirectToAction(nameof(Overview));
        }

        [HttpGet]
        public IActionResult Index()
        {
            // Sjekk om bedrift er logget inn
            var role = HttpContext.Session.GetString("Role");
            var bedriftId = HttpContext.Session.GetInt32("BedriftId");

            if (role != "Bedrift" || bedriftId == null)
                return RedirectToAction("Login", "Auth");
            

            // Hvis innlogget, vis registreringsskjema
            ViewBag.Fagområder = BuildFagomradeSelectList();
            return View(new KompetanseRegistreringViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(KompetanseRegistreringViewModel model)
        {
           
            var bedriftId = HttpContext.Session.GetInt32("BedriftId");
            if (bedriftId == null)
            {
                TempData["Message"] = "Du må være logget inn som bedrift for å registrere kompetanse.";
                TempData["MessageType"] = "warning";
                return RedirectToAction("Login", "Auth"); //
            }
            
            ViewBag.Fagområder = BuildFagomradeSelectList(model.FagområdeId);
            if (!ModelState.IsValid) return View(model);
            
            var bedrift = _context.Bedrift.FirstOrDefault(b => b.BedriftId == bedriftId.Value);
            if (bedrift == null)
                return Unauthorized("Innlogget bedrift ikke funnet.");
            
            var fagområde = _context.Fagområde
                .AsNoTracking()
                .FirstOrDefault(f => f.FagområdeId == model.FagområdeId);
            if (fagområde == null) return NotFound("Fagområde ikke funnet.");

            if (!model.KompetanseId.HasValue) return BadRequest("Kompetanse må velges.");

            var kompetanse = _context.Kompetanse
                .AsNoTracking()
                .FirstOrDefault(k => k.KompetanseId == model.KompetanseId.Value);
            if (kompetanse == null) return NotFound("Kompetanse ikke funnet.");
            
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
            return RedirectToAction(nameof(Overview));
        }

        [HttpGet]
        public JsonResult GetKompetanser(int fagområdeId)
        {
            var kompetanser = _context.Fagområde
                .AsNoTracking()
                .Include(f => f.Kompetanser)
                .Where(f => f.FagområdeId == fagområdeId)
                .SelectMany(f => f.Kompetanser)
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
                return RedirectToAction("Login", "Auth");
            
            var rader = _context.BedriftKompetanse
                .Where(bk => bk.BedriftId == bedriftId.Value /* && bk.IsActive */)
                .Include(bk => bk.Bedrift)
                .Include(bk => bk.Fagområde)
                .Include(bk => bk.Kompetanse)
                .Include(bk => bk.UnderKompetanse)
                .AsNoTracking()
                .ToList();
            
            if (rader.Count == 0)
            {
                TempData["Info"] = "Du har ikke registrert kompetanse ennå. Fyll inn skjemaet først.";
                return RedirectToAction("Index");
            }

            return View(rader);
        }
        
        public IActionResult Privacy() => View();
        public IActionResult Help() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        
    }
}
