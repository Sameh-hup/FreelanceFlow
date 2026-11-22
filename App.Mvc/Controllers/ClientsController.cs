using Microsoft.AspNetCore.Mvc;

namespace App.Mvc.Controllers;

public class ClientsController : Controller
{
    public IActionResult Index(string? search) => View();

    public IActionResult Create() => View();

    public IActionResult Edit(int id) => View();

    public IActionResult Details(int id) => View();
}
