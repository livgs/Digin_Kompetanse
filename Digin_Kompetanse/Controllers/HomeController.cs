using Digin_Kompetanse.Models.ViewModels;
using Digin_Kompetanse.data;
using Digin_Kompetanse.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Npgsql;

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


        [HttpGet("/auth/login")]
        public IActionResult Login()
        {
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

            _context.BedriftKompetanse.Remove(row);
            _context.SaveChanges();

            TempData["Success"] = "Kompetansen ble slettet.";
            return RedirectToAction(nameof(Overview));
        }

        // REGISTRERING 
        [HttpGet]
        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("Role");
            var bedriftId = HttpContext.Session.GetInt32("BedriftId");

            if (role != "Bedrift" || bedriftId == null)
                return RedirectToAction("Login", "Auth");

            ViewBag.Fagområder = BuildFagomradeSelectList();

            var model = new KompetanseRegistreringViewModel();
            model.Rader.Add(new KompetanseRadViewModel());

            return View(model);
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
                return RedirectToAction("Login", "Auth");
            }

            ViewBag.Fagområder = BuildFagomradeSelectList();

            var rader = model.Rader?
                .Where(r => r.FagområdeId.HasValue && r.KompetanseId.HasValue)
                .ToList() ?? new List<KompetanseRadViewModel>();

            if (!rader.Any())
            {
                ModelState.AddModelError(string.Empty, "Du må legge til minst én kompetanse.");
                return View(model);
            }

            if (!ModelState.IsValid)
                return View(model);

            var bedrift = _context.Bedrift.FirstOrDefault(b => b.BedriftId == bedriftId.Value);
            if (bedrift == null)
                return Unauthorized("Innlogget bedrift ikke funnet.");

            foreach (var rad in rader)
            {
                var fagområde = _context.Fagområde
                    .AsNoTracking()
                    .FirstOrDefault(f => f.FagområdeId == rad.FagområdeId);
                if (fagområde == null)
                    continue;

                var kompetanse = _context.Kompetanse
                    .AsNoTracking()
                    .FirstOrDefault(k => k.KompetanseId == rad.KompetanseId);
                if (kompetanse == null)
                    continue;

                // Finn hvilke underkompetanser som er valgt (eller ingen)
                IEnumerable<int?> underIds;
                if (rad.UnderkompetanseId != null && rad.UnderkompetanseId.Any())
                {
                    underIds = rad.UnderkompetanseId
                        .Distinct()
                        .Select(id => (int?)id);
                }
                else
                {
                    // Ingen underkompetanse valgt → lag en rad med null (kun hovedkompetanse)
                    underIds = new int?[] { null };
                }

                foreach (var underId in underIds)
                {
                    if (underId.HasValue)
                    {
                        var under = _context.UnderKompetanse
                            .AsNoTracking()
                            .FirstOrDefault(uk => uk.UnderkompetanseId == underId.Value
                                                  && uk.KompetanseId == kompetanse.KompetanseId);
                        if (under == null)
                            continue; // ugyldig kobling → hopp over
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
                            Beskrivelse = rad.Beskrivelse,
                            CreatedAt = DateTime.UtcNow,
                            IsActive = true
                        };

                        _context.BedriftKompetanse.Add(bk);
                    }
                    else
                    {
                        eksisterende.Beskrivelse = rad.Beskrivelse;
                        eksisterende.ModifiedAt = DateTime.UtcNow;
                        eksisterende.ModifiedByBedriftId = bedrift.BedriftId;
                    }
                }
            }

            _context.SaveChanges();
            return RedirectToAction(nameof(Overview));
        }

        // AJAX: hent kompetanser for fagområde
        [HttpGet]
        public JsonResult GetKompetanser(int fagområdeId)
        {
            var kompetanser = _context.Kompetanse
                .AsNoTracking()
                .Where(k => k.Fagområder.Any(f => f.FagområdeId == fagområdeId))
                .OrderBy(k => k.KompetanseKategori)
                .Select(k => new { k.KompetanseId, k.KompetanseKategori })
                .ToList();
            return Json(kompetanser);
        }


        // AJAX: hent underkompetanser for kompetanse
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

        // OVERSIKT FOR BEDRIFT
        [HttpGet]
        public IActionResult Overview()
        {
            var role = HttpContext.Session.GetString("Role");
            var bedriftId = HttpContext.Session.GetInt32("BedriftId");
            if (role != "Bedrift" || bedriftId is null)
                return RedirectToAction("Login", "Auth");

            var rader = _context.BedriftKompetanse
                .Where(bk => bk.BedriftId == bedriftId.Value)
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

            // SKAFF ALLE FAGOMRÅDER
            ViewBag.Fagområder = _context.Fagområde
                .AsNoTracking()
                .Select(f => new {
                    id = f.FagområdeId,
                    navn = f.FagområdeNavn
                })
                .OrderBy(f => f.navn)
                .ToList();

// SKAFF ALLE KOMPETANSER GRUPPERT PER FAGOMRÅDE
            ViewBag.Kompetanser = _context.Kompetanse
                .AsNoTracking()
                .Select(k => new {
                    id = k.KompetanseId,
                    navn = k.KompetanseKategori,
                    fagomradeIds = k.Fagområder.Select(f => f.FagområdeId).ToList()
                })
                .OrderBy(k => k.navn)
                .ToList();

// SKAFF ALLE UNDERKOMPETANSER
            ViewBag.Underkompetanser = _context.UnderKompetanse
                .AsNoTracking()
                .Select(u => new {
                    id = u.UnderkompetanseId,
                    navn = u.UnderkompetanseNavn,
                    kompetanseId = u.KompetanseId
                })
                .OrderBy(u => u.navn)
                .ToList();
            
            return View(rader);
        }

        public IActionResult Privacy() => View();
        public IActionResult Help() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditInline(int id, [FromBody] EditInlineDto dto)
        {
            var entry = await _context.BedriftKompetanse
                .Include(bk => bk.Bedrift)
                .Include(bk => bk.Fagområde)
                .Include(bk => bk.Kompetanse)
                .Include(bk => bk.UnderKompetanse)
                .FirstOrDefaultAsync(bk => bk.Id == id);

            if (entry == null)
                return NotFound();

            var fag = await _context.Fagområde.FirstOrDefaultAsync(f => f.FagområdeNavn == dto.Fagomrade);
            var komp = await _context.Kompetanse.FirstOrDefaultAsync(k => k.KompetanseKategori == dto.Kompetanse);
            var under = string.IsNullOrWhiteSpace(dto.Underkompetanse)
                ? null
                : await _context.UnderKompetanse.FirstOrDefaultAsync(u => u.UnderkompetanseNavn == dto.Underkompetanse);

            if (fag == null || komp == null)
                return BadRequest("Fagområde eller kompetanse ikke funnet.");

            var underIdWanted = under == null ? (int?)null : under.UnderkompetanseId;

            bool exists = await _context.BedriftKompetanse.AnyAsync(bk =>
                bk.Id != id &&
                bk.BedriftId == entry.BedriftId &&
                bk.FagområdeId == fag.FagområdeId &&
                bk.KompetanseId == komp.KompetanseId &&
                bk.UnderKompetanseId == underIdWanted &&
                bk.IsActive == true
            );

            if (exists)
            {
                return BadRequest("Denne kombinasjonen finnes allerede for denne bedriften.");
            }

            entry.FagområdeId = fag.FagområdeId;
            entry.KompetanseId = komp.KompetanseId;
            entry.UnderKompetanseId = underIdWanted;
            entry.Beskrivelse = dto.Beskrivelse;
            entry.ModifiedAt = DateTime.UtcNow;
            entry.ModifiedByBedriftId = entry.BedriftId;

            try
            {
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx && pgEx.SqlState == "23505")
            {
                return BadRequest("Denne kombinasjonen finnes allerede for denne bedriften.");
            }
        }

        public class EditInlineDto
        {
            public string Fagomrade { get; set; } = "";
            public string Kompetanse { get; set; } = "";
            public string Underkompetanse { get; set; } = "";
            public string Beskrivelse { get; set; } = "";
        }

        // AJAX for admin-filter
        [HttpGet]
        public async Task<IActionResult> GetFagomrader()
        {
            var fagomrader = await _context.Fagområde
                .Select(f => new { f.FagområdeId, f.FagområdeNavn })
                .Distinct()
                .ToListAsync();

            return Json(fagomrader);
        }

        [HttpGet]
        public async Task<IActionResult> GetKompetanserByFagomradeId(int fagomradeId)
        {
            var kompetanser = await _context.Kompetanse
                .Where(k => k.Fagområder.Any(f => f.FagområdeId == fagomradeId))
                .Select(k => new { k.KompetanseId, k.KompetanseKategori })
                .ToListAsync();

            return Json(kompetanser);
        }


        [HttpGet]
        public async Task<IActionResult> GetUnderkompetanserByKompetanseId(int kompetanseId)
        {
            var under = await _context.UnderKompetanse
                .Where(u => u.KompetanseId == kompetanseId)
                .Select(u => new { u.UnderkompetanseId, u.UnderkompetanseNavn })
                .ToListAsync();

            return Json(under);
        }

        [HttpGet]
        public async Task<IActionResult> GetKompetanserByFagomrade(string fagomrade)
        {
            if (string.IsNullOrWhiteSpace(fagomrade))
                return Json(Array.Empty<string>());

            var kompetanser = await _context.Kompetanse
                .Where(k => k.Fagområder.Any(f => f.FagområdeNavn == fagomrade))
                .Select(k => k.KompetanseKategori)
                .Distinct()
                .OrderBy(k => k)
                .ToListAsync();

            return Json(kompetanser);
        }

        [HttpGet]
        public async Task<IActionResult> GetUnderkompetanserByKompetanse(string kompetanse)
        {
            if (string.IsNullOrWhiteSpace(kompetanse))
                return Json(Array.Empty<string>());

            var under = await _context.UnderKompetanse
                .Where(u => u.Kompetanse.KompetanseKategori == kompetanse)
                .Select(u => u.UnderkompetanseNavn)
                .Distinct()
                .OrderBy(u => u)
                .ToListAsync();

            return Json(under);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddKompetanse([FromBody] KompetanseRadViewModel rad)
        {
            // Hent innlogget bedrift fra session
            var bedriftId = HttpContext.Session.GetInt32("BedriftId");
            if (bedriftId == null)
                return Unauthorized(new { message = "Ingen bedrift er logget inn." });

            if (!rad.FagområdeId.HasValue || !rad.KompetanseId.HasValue)
                return BadRequest(new { message = "Fagområde eller kompetanse mangler." });

            // Hvis flere underkompetanser, lag en rad per underkompetanse
            var underKompetanser = rad.UnderkompetanseId.Any()
                ? rad.UnderkompetanseId.Select(i => (int?)i).ToList()
                : new List<int?>() { null };
            

            var nyeRader = new List<BedriftKompetanse>();

            foreach (var uk in underKompetanser)
            {
                var nyRad = new BedriftKompetanse
                {
                    BedriftId = bedriftId.Value,
                    FagområdeId = rad.FagområdeId.Value,
                    KompetanseId = rad.KompetanseId.Value,
                    UnderKompetanseId = uk,
                    Beskrivelse = rad.Beskrivelse
                };
                nyeRader.Add(nyRad);
                _context.BedriftKompetanse.Add(nyRad);
            }

            await _context.SaveChangesAsync();

            // Returner første rad (eller tilpass for å returnere alle)
            var førsteRad = nyeRader.First();
            var fagNavn = (await _context.Fagområde.FindAsync(førsteRad.FagområdeId))?.FagområdeNavn;
            var kompNavn = (await _context.Kompetanse.FindAsync(førsteRad.KompetanseId))?.KompetanseKategori;
            var underNavn = førsteRad.UnderKompetanseId.HasValue
                ? (await _context.UnderKompetanse.FindAsync(førsteRad.UnderKompetanseId.Value))?.UnderkompetanseNavn
                : null;

            return Json(new
            {
                id = førsteRad.Id,
                fagomradeNavn = fagNavn,
                kompetanseNavn = kompNavn,
                underkompetanseNavn = underNavn != null ? new[] { underNavn } : null,
                createdAt = førsteRad.CreatedAt.ToLocalTime().ToString("dd.MM.yyyy")
            });
        }
    }
}