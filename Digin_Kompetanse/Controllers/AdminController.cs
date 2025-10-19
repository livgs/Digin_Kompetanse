using Digin_Kompetanse.data;
using Digin_Kompetanse.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace Digin_Kompetanse.Controllers;

public class AdminController : Controller
{
    private readonly KompetanseContext _context;


    public AdminController(KompetanseContext context)
    {
        _context = context;
    }
    
    [HttpGet]
    public IActionResult AdminLogin() => View("AdminLogin");
    
    [HttpPost]
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
            return RedirectToAction("Login");

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
    public IActionResult Logout()
    {
        HttpContext.Session.Remove("Role");
        HttpContext.Session.Remove("AdminEmail");
        return RedirectToAction("Login");
    }

}