using Microsoft.AspNetCore.Mvc;

namespace App.Mvc.Controllers;

public class AdminController : Controller
{
    public IActionResult Index() => View();

    [HttpPost]
    public IActionResult ToggleUserStatus(int userId) => Json(new { success = true });
}
