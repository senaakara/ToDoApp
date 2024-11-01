using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;

public class AccountController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(Register model)
    {
        if (ModelState.IsValid)
        {
            var user = new User
            {
                UserName = model.UserName,
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            // Kullanıcıyı Identity'nin kendi hashleme ve doğrulama mekanizmasıyla oluşturun
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Kullanıcıya varsayılan olarak "User" rolünü ekleyin
                await _userManager.AddToRoleAsync(user, "User");

                // Kullanıcıyı oturum açmış şekilde sisteme alın
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            // Hataları ModelState'e ekleyin
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        // Model geçerli değilse veya kullanıcı oluşturulurken hata varsa, formu tekrar göster
        return View(model);
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(Login model)
    {
        if (ModelState.IsValid)
        {
            // Kullanıcıyı e-posta ile bul
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                // Kullanıcı adı ve şifre ile giriş yap
                var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "Hesabınız geçici olarak kilitlendi.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Giriş başarısız. Lütfen bilgilerinizi kontrol edin.");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Kullanıcı bulunamadı.");
            }
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login", "Account");
    }
}
