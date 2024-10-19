using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;

public class UserController : Controller
{
    private readonly IUserRepository _repository;

    public UserController(IUserRepository repository)
    {
        _repository = repository;
    }

    // Kullanıcıları listelemek için sayfa
    public async Task<IActionResult> Index()
    {
        var users = await _repository.GetAllAsync();
        return View(users);  // Razor view'a 'users' verisi gönderilir
    }

    // Kullanıcı detay sayfası
    public async Task<IActionResult> Details(int id)
    {
        var user = await _repository.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return View(user);  // Razor view'a 'user' verisi gönderilir
    }

    // Yeni kullanıcı oluşturma sayfası (GET)
    public IActionResult Create()
    {
        return View();
    }

    // Yeni kullanıcı oluşturma (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(User user)
    {
        if (ModelState.IsValid)
        {
            await _repository.AddAsync(user); // Veritabanına kullanıcı eklenir
            return RedirectToAction(nameof(Index)); // Başarılı olursa listeye geri yönlendirir
        }

        return View(user); // Hatalıysa form tekrar yüklenir
    }


    // Kullanıcı düzenleme sayfası (GET)
    public async Task<IActionResult> Edit(int id)
    {
        var user = await _repository.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return View(user);  // Kullanıcı düzenleme sayfasına 'user' verisi gönderilir
    }

    // Kullanıcı düzenleme (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, User user)
    {
        if (id != user.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            await _repository.UpdateAsync(user);
            return RedirectToAction(nameof(Index));  // Düzenleme sonrası listeye dön
        }

        return View(user);  // Hatalıysa aynı sayfayı döndür ve hataları göster
    }

    // Kullanıcı silme sayfası (GET)
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _repository.GetByIdAsync(id);
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
        var user = await _repository.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        await _repository.DeleteAsync(id);
        return RedirectToAction(nameof(Index));  // Silme işlemi sonrası listeye dön
    }
}
