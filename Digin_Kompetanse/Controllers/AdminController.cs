using Digin_Kompetanse.data;
using Digin_Kompetanse.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace Digin_Kompetanse.Controllers;

public class AdminController : Controller
{
    private readonly KompetanseContext _context;
    private readonly ILogger<HomeController> _logger;


    public AdminController(KompetanseContext context, ILogger<HomeController> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    [HttpGet]
    public IActionResult AdminLogin() => View("AdminLogin");
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AdminLogin(string email, string password)
    {
        // Hardkodet admin 
        const string hardcodedEmail = "digin@dgn.no";
        const string hardcodedPasswordHash = "$2a$11$TTl31f/y3FAKOR3n0hwq4uJR6Q0u.mamXVDGIcPGmGoJNx0/SiFUO";
        
        // verifiser innlogging
        if (email == hardcodedEmail && BCrypt.Net.BCrypt.Verify(password, hardcodedPasswordHash))
        {
            HttpContext.Session.SetString("Role", "Admin");
            HttpContext.Session.SetString("AdminEmail", email); // valgfritt for visning i UI
            return RedirectToAction("AdminDashboard");
        }

        // Feil brukernavn/passord
        ViewBag.Error = "Feil e-post eller passord.";
        return View();
    }

    [HttpGet]
    public IActionResult AdminDashboard()
    {
        // Sjekk at brukeren er logget inn som admin
        if (HttpContext.Session.GetString("Role") != "Admin")
            return RedirectToAction("AdminLogin");

        try
        {
            // Hent data fra databasen
            var viewModel = _context.BedriftKompetanse
                .Include(bk => bk.Bedrift)
                .Include(bk => bk.Fagområde)
                .Include(bk => bk.Kompetanse)
                .AsNoTracking()
                .Select(bk => new AdminViewModel
                {
                    BedriftId = bk.BedriftId,
                    BedriftNavn = bk.Bedrift!.BedriftNavn,
                    Epost = bk.Bedrift!.BedriftEpost,
                    Fagområde = bk.Fagområde!.FagområdeNavn!,
                    KompetanseKategori = bk.Kompetanse!.KompetanseKategori!
                })
                .OrderBy(x => x.BedriftNavn)
                .ThenBy(x => x.Fagområde)
                .ThenBy(x => x.KompetanseKategori)
                .ToList();
            
            return View(viewModel);
        }
        catch
        {
            ViewBag.Error = "Kunne ikke hente data fra databasen.";
            return View(new List<AdminViewModel>());
        }
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult LogoutAdmin()
    {
        // Fjern admin-session
        HttpContext.Session.Remove("AdminEmail");
        HttpContext.Session.Remove("Role");
        return RedirectToAction("AdminLogin");
    }

    
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