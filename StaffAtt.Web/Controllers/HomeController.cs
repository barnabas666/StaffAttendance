using Microsoft.AspNetCore.Mvc;
using StaffAtt.Web.Models;
using System.Diagnostics;

namespace StaffAtt.Web.Controllers;
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Serves the default view for the current controller.
    /// </summary>
    /// <returns>An <see cref="IActionResult"/> that renders the Index view.</returns>
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>
    /// Handles requests to the "About" page with info about this app and returns the corresponding view.
    /// </summary>
    /// <returns>An <see cref="IActionResult"/> that renders the "About" view.</returns>
    public IActionResult About()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
