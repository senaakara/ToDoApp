using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

public class UserController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;

    public UserController(UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    // Kullanıcıları listelemek için sayfa
    public async Task<IActionResult> Index()
    {
        var users = _userManager.Users;
        return View(users);  // Razor view'a 'users' verisi gönderilir
    }

    // Kullanıcı detay sayfası
    public async Task<IActionResult> Details(int id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            return NotFound();
        }

        var roles = await _userManager.GetRolesAsync(user);
        ViewBag.Roles = roles;

        return View(user);  // Razor view'a 'user' verisi ve rolleri gönderilir
    }

    // Yeni kullanıcı oluşturma sayfası (GET)
    public IActionResult Create()
    {
        return View();
    }

    // Yeni kullanıcı oluşturma (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(User user, string password)
    {
        if (ModelState.IsValid)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                // Varsayılan olarak "User" rolünü atayın
                await _userManager.AddToRoleAsync(user, "User");
                return RedirectToAction(nameof(Index));
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
        }

        return View(user);
    }

    // Kullanıcı düzenleme sayfası (GET)
    public async Task<IActionResult> Edit(int id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            return NotFound();
        }

        var roles = await _userManager.GetRolesAsync(user);
        ViewBag.Roles = roles;

        return View(user);  // Kullanıcı düzenleme sayfasına 'user' verisi ve rolleri gönderilir
    }

    // Kullanıcı düzenleme (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, User updatedUser)
    {
        if (id != updatedUser.Id)
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            user.UserName = updatedUser.UserName;
            user.Email = updatedUser.Email;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
        }

        return View(updatedUser);
    }

    // Kullanıcı silme sayfası (GET)
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            return NotFound();
        }

        return View(user);  // Silme sayfasına 'user' verisi gönderilir
    }

    // Kullanıcı silme işlemi (POST)
    [HttpPost, ActionName("DeleteConfirmed")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            return NotFound();
        }

        await _userManager.DeleteAsync(user);
        return RedirectToAction(nameof(Index));
    }
}
