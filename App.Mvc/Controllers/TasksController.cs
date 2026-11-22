using Microsoft.AspNetCore.Mvc;

namespace App.Mvc.Controllers;

public class TasksController : Controller
{
    [HttpPost]
    public IActionResult Create() => Ok();

    [HttpPost]
    public IActionResult UpdateStatus(int id, string status) => Json(new { success = true });
}
