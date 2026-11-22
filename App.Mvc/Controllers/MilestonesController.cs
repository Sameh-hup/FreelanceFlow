using Microsoft.AspNetCore.Mvc;

namespace App.Mvc.Controllers;

public class MilestonesController : Controller
{
    [HttpPost]
    public IActionResult Create() => Ok();
}
