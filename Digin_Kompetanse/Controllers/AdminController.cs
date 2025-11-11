using Digin_Kompetanse.data;
using Digin_Kompetanse.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Digin_Kompetanse.Controllers
{
    public class AdminController : Controller
    {
        private readonly KompetanseContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            KompetanseContext context,
            ILogger<AdminController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult AdminLogin()
        {
            if (HttpContext.Session.GetString("Role") == "Admin")
                return RedirectToAction(nameof(AdminDashboard));

            return View("AdminLogin");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminLogin(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Skriv inn både e-post og passord.";
                return View("AdminLogin");
            }

            var inputEmail = email.Trim().ToLowerInvariant();
            var inputPassword = password ?? string.Empty;

            try
            {
                var admin = await _context.Admin
                    .AsNoTracking()
                    .FirstOrDefaultAsync(a => a.AdminEpost.ToLower() == inputEmail);

                if (admin != null && BCrypt.Net.BCrypt.Verify(inputPassword, admin.AdminPassordHash))
                {
                    HttpContext.Session.SetString("Role", "Admin");
                    HttpContext.Session.SetString("AdminEmail", admin.AdminEpost);
                    return RedirectToAction(nameof(AdminDashboard));
                }

                ViewBag.Error = "Feil e-post eller passord.";
                return View("AdminLogin");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Feil under admin-innlogging.");
                ViewBag.Error = "Noe gikk galt under innlogging. Prøv igjen senere.";
                return View("AdminLogin");
            }
        }

        [HttpGet]
        public IActionResult AdminDashboard(string? fagomrade, string? kompetanse, string? underkompetanse)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction(nameof(AdminLogin));

            try
            {
                var query = _context.BedriftKompetanse
                    .Include(bk => bk.Bedrift)
                    .Include(bk => bk.Fagområde)
                    .Include(bk => bk.Kompetanse)
                    .Include(bk => bk.UnderKompetanse)
                    .AsNoTracking()
                    .Select(bk => new AdminViewModel
                    {
                        BedriftId = bk.BedriftId,
                        BedriftNavn = bk.Bedrift != null ? bk.Bedrift.BedriftNavn : "(ukjent)",
                        Epost = bk.Bedrift != null ? bk.Bedrift.BedriftEpost : "-",
                        Fagområde = bk.Fagområde != null ? bk.Fagområde.FagområdeNavn ?? "-" : "-",
                        KompetanseKategori = bk.Kompetanse != null ? bk.Kompetanse.KompetanseKategori ?? "-" : "-",
                        UnderKompetanse = bk.UnderKompetanse != null
                            ? bk.UnderKompetanse.UnderkompetanseNavn ?? "-"
                            : "-",
                        Beskrivelse = string.IsNullOrWhiteSpace(bk.Beskrivelse) ? "-" : bk.Beskrivelse
                    })
                    .AsQueryable();

                if (!string.IsNullOrEmpty(fagomrade))
                    query = query.Where(x => x.Fagområde == fagomrade);

                if (!string.IsNullOrEmpty(kompetanse))
                    query = query.Where(x => x.KompetanseKategori == kompetanse);

                if (!string.IsNullOrEmpty(underkompetanse))
                    query = query.Where(x => x.UnderKompetanse == underkompetanse);

                ViewBag.Fagomrader = _context.Fagområde
                    .Select(f => f.FagområdeNavn!)
                    .Distinct()
                    .OrderBy(f => f)
                    .ToList();

                ViewBag.Kompetanser = _context.Kompetanse
                    .Select(k => k.KompetanseKategori!)
                    .Distinct()
                    .OrderBy(k => k)
                    .ToList();

                ViewBag.Underkompetanser = _context.UnderKompetanse
                    .Where(u => !string.IsNullOrEmpty(u.UnderkompetanseNavn))
                    .Select(u => u.UnderkompetanseNavn!)
                    .Distinct()
                    .OrderBy(u => u)
                    .ToList();

                var viewModel = query
                    .OrderBy(x => x.BedriftNavn)
                    .ThenBy(x => x.Fagområde)
                    .ThenBy(x => x.KompetanseKategori)
                    .ToList();

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Feil under lasting av admin dashboard.");
                ViewBag.Error = "Kunne ikke hente data fra databasen.";
                return View(new List<AdminViewModel>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> ExportCsv(string? fagomrade, string? kompetanse, string? underkompetanse)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("AdminLogin");

            try
            {
                var query = _context.BedriftKompetanse
                    .Include(bk => bk.Bedrift)
                    .Include(bk => bk.Fagområde)
                    .Include(bk => bk.Kompetanse)
                    .Include(bk => bk.UnderKompetanse)
                    .AsNoTracking()
                    .Select(bk => new AdminViewModel
                    {
                        BedriftNavn = bk.Bedrift != null ? bk.Bedrift.BedriftNavn : "(ukjent)",
                        Epost = bk.Bedrift != null ? bk.Bedrift.BedriftEpost : "-",
                        Fagområde = bk.Fagområde != null ? bk.Fagområde.FagområdeNavn ?? "-" : "-",
                        KompetanseKategori = bk.Kompetanse != null ? bk.Kompetanse.KompetanseKategori ?? "-" : "-",
                        UnderKompetanse = bk.UnderKompetanse != null
                            ? bk.UnderKompetanse.UnderkompetanseNavn ?? "-"
                            : "-",
                        Beskrivelse = string.IsNullOrWhiteSpace(bk.Beskrivelse) ? "-" : bk.Beskrivelse
                    })
                    .AsQueryable();

                if (!string.IsNullOrEmpty(fagomrade))
                    query = query.Where(x => x.Fagområde == fagomrade);

                if (!string.IsNullOrEmpty(kompetanse))
                    query = query.Where(x => x.KompetanseKategori == kompetanse);

                if (!string.IsNullOrEmpty(underkompetanse))
                    query = query.Where(x => x.UnderKompetanse == underkompetanse);

                var data = await query
                    .OrderBy(x => x.BedriftNavn)
                    .ThenBy(x => x.Fagområde)
                    .ThenBy(x => x.KompetanseKategori)
                    .ToListAsync();

                var sb = new StringBuilder();
                sb.AppendLine("Bedrift;E-post;Fagområde;Kompetanse;Underkompetanse;Beskrivelse");

                foreach (var item in data)
                {
                    sb.AppendLine(
                        $"{item.BedriftNavn};{item.Epost};{item.Fagområde};{item.KompetanseKategori};{item.UnderKompetanse};{item.Beskrivelse}");
                }

                var bytes = Encoding.UTF8.GetPreamble()
                    .Concat(Encoding.UTF8.GetBytes(sb.ToString()))
                    .ToArray();

                return File(bytes, "text/csv", "innsendinger.csv");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Feil ved eksport av CSV.");
                TempData["Error"] = "Kunne ikke eksportere CSV.";
                return RedirectToAction(nameof(AdminDashboard));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LogoutAdmin()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("AdminLogin");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteBedrift(int id)
        {
            try
            {
                var bedrift = _context.Bedrift
                    .Include(b => b.BedriftKompetanser)
                    .FirstOrDefault(b => b.BedriftId == id);

                if (bedrift == null)
                {
                    TempData["Error"] = "Fant ikke bedriften.";
                    return RedirectToAction(nameof(AdminDashboard));
                }

                if (bedrift.BedriftKompetanser?.Any() == true)
                    _context.BedriftKompetanse.RemoveRange(bedrift.BedriftKompetanser);

                _context.Bedrift.Remove(bedrift);
                _context.SaveChanges();

                TempData["Success"] = "Bedriften og tilhørende kompetanser ble slettet.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Feil ved sletting av bedrift.");
                TempData["Error"] = "Noe gikk galt under sletting.";
            }

            return RedirectToAction(nameof(AdminDashboard));
        }

        [HttpGet]
        public JsonResult GetKompetanserByFagomrade(string fagomradeNavn)
        {
            if (string.IsNullOrWhiteSpace(fagomradeNavn))
                return Json(new List<object>());
            var kompetanser = (
                    from f in _context.Fagområde
                    from k in f.Kompetanser
                    where f.FagområdeNavn == fagomradeNavn
                    select new { k.KompetanseKategori }
                )
                .Distinct()
                .OrderBy(k => k.KompetanseKategori)
                .ToList();
            return Json(kompetanser);
        }

        [HttpGet]
        public JsonResult GetUnderkompetanserByKompetanse(string kompetanseNavn)
        {
            if (string.IsNullOrWhiteSpace(kompetanseNavn))
                return Json(new List<object>());
            var underkompetanser = (
                    from k in _context.Kompetanse
                    from uk in k.UnderKompetanser
                    where k.KompetanseKategori == kompetanseNavn
                    select new { uk.UnderkompetanseNavn }
                )
                .Distinct()
                .OrderBy(uk => uk.UnderkompetanseNavn)
                .ToList();
            return Json(underkompetanser);
        }
    }
}
