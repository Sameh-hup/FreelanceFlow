using Microsoft.AspNetCore.Mvc;

namespace App.Mvc.Controllers;

public class PortalController : Controller
{
    public IActionResult Index() => View();

    public IActionResult Projects() => View();

    public IActionResult Invoices() => View();
}
