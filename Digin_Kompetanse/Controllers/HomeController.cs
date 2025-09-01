using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Digin_Kompetanse.Models;

namespace Digin_Kompetanse.Controllers;

public class HomeController : Controller
{
    //Midlertidig database i minnet
    private static List<KompetanseModel> _kompetanser = new();
   
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Index(string bedriftNavn, string bedriftEpost, string fagområdeNavn, string kompetanseNavn, string beskrivelse)
    {
        //Lag modellene
        var bedrift = new BedriftModel { Navn = bedriftNavn, Epost = bedriftEpost };
        var fagområde = new FagområdeModel { Navn = fagområdeNavn };
        var kompetanse = new KompetanseModel
        {
            Bedrift = bedrift,
            Fagområde = fagområde,
            KompetanseNavn = kompetanseNavn,
            Beskrivelse = beskrivelse
        };
        
        //Legg til i listen
        _kompetanser.Add(kompetanse);
        
        //send brukeren til oversikten
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
