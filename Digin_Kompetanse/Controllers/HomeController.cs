using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Digin_Kompetanse.Models;
using Microsoft.EntityFrameworkCore;

namespace Digin_Kompetanse.Controllers;

public class HomeController : Controller
{
    private static List<KompetanseModel> _kompetanser = new();

    public HomeController() { }

    [HttpGet]
    public IActionResult Index() => View();

    [HttpPost]
    public IActionResult Index(string bedriftNavn, string bedriftEpost, string fagområdeNavn, string kompetanseNavn, string beskrivelse)
    {
        var bedrift = new BedriftModel { Navn = bedriftNavn, Epost = bedriftEpost };
        var fagområde = new FagområdeModel { Navn = fagområdeNavn };
        var kompetanse = new KompetanseModel
        {
            KompetanseId = _kompetanser.Count + 1,
            Navn = kompetanseNavn,
            Bedrift = bedrift,
            Fagområde = fagområde,
            Beskrivelse = beskrivelse
        };

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