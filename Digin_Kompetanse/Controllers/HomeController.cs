using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Digin_Kompetanse.Models;

namespace Digin_Kompetanse.Controllers;

public class HomeController : Controller
{
    // Midlertidig liste i minnet
    private static List<Kompetanse> _kompetanser = new();

    [HttpGet]
    public IActionResult Index() => View();

    [HttpPost]
    public IActionResult Index(string bedriftNavn, string bedriftEpost, string fagområdeNavn, string kompetanseNavn, string kompetanseKategori, string beskrivelse)
    {
        // Opprett Bedrift
        var bedrift = new Bedrift
        {
            BedriftId = _kompetanser.Count + 1,
            BedriftNavn = bedriftNavn,
            BedriftEpost = bedriftEpost
        };

        // Opprett Fagområde
        var fagområde = new Fagområde
        {
            FagområdeId = _kompetanser.Count + 1,
            FagområdeNavn = fagområdeNavn,
            Bedrift = bedrift
        };

        // Opprett Kompetanse
        var kompetanse = new Kompetanse
        {
            KompetanseId = _kompetanser.Count + 1,
            KompetanseKategori = kompetanseKategori
        };

        // Legg fagområde til Kompetanse-relasjonen
        kompetanse.Fagområdes.Add(fagområde);

        // Legg kompetanse til global liste
        _kompetanser.Add(kompetanse);

        return RedirectToAction("Overview");
    }

    public IActionResult Overview()
    {
        return View(_kompetanser);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Help()
    {
        return View();
    }

    public IActionResult AdminView()
    {
        return View();
    }

    public IActionResult AdminOverview()
    {
        return View(_kompetanser);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}