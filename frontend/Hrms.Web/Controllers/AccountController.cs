using Hrms.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Hrms.Web.Controllers;

public class AccountController(
    UserManager<ApplicationUser> users,
    SignInManager<ApplicationUser> signIn,
    ILogger<AccountController> logger) : Controller
{
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string email, string password, bool rememberMe, string? returnUrl = null)
    {
        var user = await users.FindByEmailAsync(email);
        if (user is null)
        {
            ModelState.AddModelError("", "Неверный email или пароль.");
            return View((object)(email, rememberMe, returnUrl)!);
        }

        var ok = await signIn.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);
        if (!ok.Succeeded)
        {
            ModelState.AddModelError("", "Неверный email или пароль.");
            return View((object)(email, rememberMe, returnUrl)!);
        }

        await signIn.SignInAsync(user, rememberMe);
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(string email, string password, string displayName)
    {
        var user = new ApplicationUser { UserName = email, Email = email, DisplayName = displayName };
        var result = await users.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            foreach (var e in result.Errors) ModelState.AddModelError("", e.Description);
            return View();
        }
        await users.AddToRoleAsync(user, "HR");
        await signIn.SignInAsync(user, isPersistent: false);
        logger.LogInformation("New user {Email} registered", email);
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await signIn.SignOutAsync();
        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult AccessDenied() => View();
}

[Authorize]
public class ManageController(UserManager<ApplicationUser> users) : Controller
{
    public async Task<IActionResult> Index()
    {
        var all = users.Users.ToList();
        var model = new List<(ApplicationUser User, IList<string> Roles)>();
        foreach (var u in all)
            model.Add((u, await users.GetRolesAsync(u)));
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ToggleAdmin(string userId)
    {
        var user = await users.FindByIdAsync(userId);
        if (user is null) return NotFound();
        if (await users.IsInRoleAsync(user, "Admin"))
            await users.RemoveFromRoleAsync(user, "Admin");
        else
            await users.AddToRoleAsync(user, "Admin");
        return RedirectToAction(nameof(Index));
    }
}
