using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;

namespace Digin_Kompetanse.Controllers;

public class AdminController2 : Controller
{
    // Hardkodet admin-bruker (for testing / utvikling)
    private const string AdminEmail = "admin@digin.no";
    private const string AdminPasswordHash = "$2a$11$Z9GrZ7bE6qA4yx/fE6UoXehxhhX8iUMv.Jy8UKchG0XH6ge44gfL2"; // erstatt med din hash

    
    [HttpGet]
    public IActionResult Login()
    {
        // Hvis allerede logget inn som admin, send rett til dashboard
        if (HttpContext.Session.GetString("Role") == "Admin")
            return RedirectToAction("Dashboard");

        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(string email, string password)
    {
        // Sjekk e-post først
        if (!string.Equals(email, AdminEmail, StringComparison.OrdinalIgnoreCase))
        {
            ViewBag.Error = "Feil e-postadresse.";
            return View();
        }

        // Verifiser passord mot lagret hash
        bool isValid = BCrypt.Net.BCrypt.Verify(password, AdminPasswordHash);
        if (!isValid)
        {
            ViewBag.Error = "Feil passord.";
            return View();
        }
        
        HttpContext.Session.SetString("Role", "Admin");
        HttpContext.Session.SetString("AdminEmail", AdminEmail);
        
        return RedirectToAction("Dashboard");
    }

    
    public IActionResult Dashboard()
    {
        var role = HttpContext.Session.GetString("Role");
        if (role != "Admin")
            return RedirectToAction("Login", "AdminController2");
        
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        // Fjern admin-session
        HttpContext.Session.Remove("AdminEmail");
        HttpContext.Session.Remove("Role");
        HttpContext.Session.Clear();

        // Send brukeren tilbake til login-siden
        return RedirectToAction("Login");
    }
}
