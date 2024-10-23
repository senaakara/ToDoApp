using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;


public class AccountController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    // GET: Account/Register
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    // POST: Account/Register
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
                Role = "User",  // Varsayılan rolü atıyoruz
                SecurityStamp = Guid.NewGuid().ToString(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password) // Şifreyi hashleyin
            };

            var result = await _userManager.CreateAsync(user, user.PasswordHash);

            if (result.Succeeded)
            {
                // Kullanıcı başarılı bir şekilde oluşturulduysa giriş işlemini yapıyoruz
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "ToDoItem");  // Kayıt sonrası yönlendirme
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        // Eğer model hataları varsa geri döner
        return View(model);
    }

    // GET: Account/Login
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    // POST: Account/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(Login model)
    {
        if (ModelState.IsValid)
        {
            // Kullanıcıyı e-posta veya kullanıcı adı ile buluyoruz
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "ToDoItem");  // Giriş sonrası yönlendirme
            }

            ModelState.AddModelError(string.Empty, "Giriş başarısız. Lütfen bilgilerinizi kontrol edin.");
        }

        return View(model);
    }

    // POST: Account/Logout
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    // GET: Account/AccessDenied
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}
