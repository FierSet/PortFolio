using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PortFolio.Models;

namespace Portfolio.Controllers;

public class PortfolioController : Controller
{
    public IActionResult Portfolio()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}