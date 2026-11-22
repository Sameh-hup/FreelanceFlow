using Microsoft.AspNetCore.Mvc;

namespace App.Mvc.Controllers;

public class InvoicesController : Controller
{
    public IActionResult Index(string? search) => View();

    public IActionResult Create() => View();

    public IActionResult Details(int id) => View();

    [HttpPost]
    public IActionResult AddPayment(int invoiceId, decimal amount) => Ok();
}
