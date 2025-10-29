using Digin_Kompetanse.data;
using Digin_Kompetanse.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using System.Text;

namespace Digin_Kompetanse.Controllers
{
    public class AdminController : Controller
    {
        private readonly KompetanseContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(KompetanseContext context, ILogger<AdminController> logger)
        {
            _context = context;
            _logger = logger;
        }
        
        // LOGIN
        [HttpGet]
        public IActionResult AdminLogin() => View("AdminLogin");

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AdminLogin(string email, string password)
        {
            const string hardcodedEmail = "digin@dgn.no";
            const string hardcodedPasswordHash = "$2a$11$TTl31f/y3FAKOR3n0hwq4uJR6Q0u.mamXVDGIcPGmGoJNx0/SiFUO";

            if (email == hardcodedEmail && BCrypt.Net.BCrypt.Verify(password, hardcodedPasswordHash))
            {
                HttpContext.Session.SetString("Role", "Admin");
                HttpContext.Session.SetString("AdminEmail", email);
                return RedirectToAction("AdminDashboard");
            }

            ViewBag.Error = "Feil e-post eller passord.";
            return View();
        }

        
        // DASHBOARD
        [HttpGet]
        public IActionResult AdminDashboard(string? fagomrade, string? kompetanse)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("AdminLogin");

            try
            {
                var query = _context.BedriftKompetanse
                    .Include(bk => bk.Bedrift)
                    .Include(bk => bk.Fagområde)
                    .Include(bk => bk.Kompetanse)
                    .Include(bk => bk.UnderKompetanse) // ✅ include sub-competence
                    .AsNoTracking()
                    .Select(bk => new AdminViewModel
                    {
                        BedriftId = bk.BedriftId,
                        BedriftNavn = bk.Bedrift!.BedriftNavn,
                        Epost = bk.Bedrift!.BedriftEpost,
                        Fagområde = bk.Fagområde!.FagområdeNavn!,
                        KompetanseKategori = bk.Kompetanse!.KompetanseKategori!,
                        UnderKompetanse = bk.UnderKompetanse != null
                            ? bk.UnderKompetanse.UnderkompetanseNavn
                            : "-",
                        Beskrivelse = string.IsNullOrWhiteSpace(bk.Beskrivelse) ? "-" : bk.Beskrivelse
                    })
                    .AsQueryable();

                if (!string.IsNullOrEmpty(fagomrade))
                    query = query.Where(x => x.Fagområde == fagomrade);

                if (!string.IsNullOrEmpty(kompetanse))
                    query = query.Where(x => x.KompetanseKategori == kompetanse);

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
                _logger.LogError(ex, "Kunne ikke hente data fra databasen.");
                ViewBag.Error = "Kunne ikke hente data fra databasen.";
                return View(new List<AdminViewModel>());
            }
        }
        
        // CSV EKSPORT
       [HttpGet]
public async Task<IActionResult> ExportCsv(string? fagomrade, string? kompetanse, string? underkompetanse)
{
    if (HttpContext.Session.GetString("Role") != "Admin")
        return RedirectToAction("AdminLogin");

    var query = _context.BedriftKompetanse
        .Include(bk => bk.Bedrift)
        .Include(bk => bk.Fagområde)
        .Include(bk => bk.Kompetanse)
        .Include(bk => bk.UnderKompetanse)
        .AsNoTracking()
        .Select(bk => new AdminViewModel
        {
            BedriftId = bk.BedriftId,
            BedriftNavn = bk.Bedrift!.BedriftNavn,
            Epost = bk.Bedrift!.BedriftEpost,
            Fagområde = bk.Fagområde!.FagområdeNavn!,
            KompetanseKategori = bk.Kompetanse!.KompetanseKategori!,
            UnderKompetanse = bk.UnderKompetanse != null
                ? bk.UnderKompetanse.UnderkompetanseNavn
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
        string bedrift = item.BedriftNavn?.Replace(";", ",") ?? "";
        string epost = item.Epost?.Replace(";", ",") ?? "";
        string fag = item.Fagområde?.Replace(";", ",") ?? "";
        string komp = item.KompetanseKategori?.Replace(";", ",") ?? "";
        string under = item.UnderKompetanse?.Replace(";", ",") ?? "";
        string beskrivelse = item.Beskrivelse?.Replace(";", ",") ?? "";

        sb.AppendLine($"\"{bedrift}\";\"{epost}\";\"{fag}\";\"{komp}\";\"{under}\";\"{beskrivelse}\"");
    }

    var bytes = Encoding.UTF8.GetPreamble()
        .Concat(Encoding.UTF8.GetBytes(sb.ToString()))
        .ToArray();

    return File(bytes, "text/csv; charset=utf-8", "innsendinger.csv");
}

        
        // LOGOUT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LogoutAdmin()
        {
            HttpContext.Session.Remove("AdminEmail");
            HttpContext.Session.Remove("Role");
            return RedirectToAction("AdminLogin");
        }
        
        // SLETT BEDRIFT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteBedrift(int id)
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
            return RedirectToAction(nameof(AdminDashboard));
        }
    }
}
