using Microsoft.AspNetCore.Mvc;

namespace App.Mvc.Controllers;

public class AccountController : Controller
{
    public IActionResult Login() => View();

    public IActionResult Register() => View();

    public IActionResult ChangePassword() => View();

    [HttpPost]
    public IActionResult Logout() => RedirectToAction("Login");
}
