using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
[Authorize]
public class ToDoItemController : Controller
{
    private readonly IToDoItemRepository _repository;
    private readonly UserManager<User> _userManager; // Kullanıcıyı almak için

    public ToDoItemController(IToDoItemRepository repository, UserManager<User> userManager)
    {
        _repository = repository;
        _userManager = userManager;
    }

    // GET: ToDoItem
    public async Task<IActionResult> Index()
    {
        // Giriş yapan kullanıcının ID'sini al
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account"); // Eğer giriş yapılmadıysa login'e yönlendir
        }

        // Sadece bu kullanıcının to-do'larını getirin
        var toDoItems = await _repository.GetToDoItemsByUserIdAsync(user.Id);
        return View(toDoItems);
    }

    // GET: ToDoItem/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _userManager.GetUserAsync(User);
        var toDoItem = await _repository.GetByIdAsync(id.Value);

        if (toDoItem == null || toDoItem.UserId != user.Id)
        {
            return NotFound(); // Erişim engellenir
        }

        return View(toDoItem);
    }

    // GET: ToDoItem/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: ToDoItem/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ToDoItem toDoItem)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account"); // Giriş yapılmadıysa yönlendirin
        }

        if (ModelState.IsValid)
        {   
            if (toDoItem.DueDate.HasValue)
            {
                // DueDate'i UTC'ye çevir
                toDoItem.DueDate = DateTime.SpecifyKind(toDoItem.DueDate.Value, DateTimeKind.Utc);
            }
            // Giriş yapan kullanıcının ID'sini toDoItem'a bağla
            toDoItem.UserId = user.Id;

            await _repository.AddAsync(toDoItem);
            return RedirectToAction(nameof(Index));
        }

        return View(toDoItem);
    }

    // GET: ToDoItem/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var toDoItem = await _repository.GetByIdAsync(id.Value);
        if (toDoItem == null)
        {
            return NotFound();
        }

        // Giriş yapan kullanıcıyı al
        var user = await _userManager.GetUserAsync(User);

        // Kullanıcı bu görevi düzenleyebiliyor mu kontrol et
        if (user == null || toDoItem.UserId != user.Id)
        {
            return Unauthorized(); // Yetkisi yoksa
        }

        return View(toDoItem);
    }

    // POST: ToDoItem/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ToDoItem toDoItem)
    {
        if (id != toDoItem.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {   

            if (toDoItem.DueDate.HasValue)
            {
                // DueDate'i UTC'ye çevir
                toDoItem.DueDate = DateTime.SpecifyKind(toDoItem.DueDate.Value, DateTimeKind.Utc);
            }
            

            try
            {
                // Giriş yapan kullanıcıyı al
                var user = await _userManager.GetUserAsync(User);
                
                // Kullanıcı bu görevi düzenleyebiliyor mu kontrol et
                if (user == null || toDoItem.UserId != user.Id)
                {
                    return Unauthorized(); // Yetkisi yoksa
                }

                // Görevi güncelle
                await _repository.UpdateAsync(toDoItem);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _repository.ExistsAsync(toDoItem.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }

        return View(toDoItem);
    }

    // GET: ToDoItem/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _userManager.GetUserAsync(User);
        var toDoItem = await _repository.GetByIdAsync(id.Value);

        if (toDoItem == null || toDoItem.UserId != user.Id)
        {
            return NotFound(); // Kendi ToDoItem'ı değilse erişim engellenir
        }

        return View(toDoItem);
    }

    // POST: ToDoItem/DeleteConfirmed/5
    [HttpPost, ActionName("DeleteConfirmed")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        // ToDoItem'ı alıyoruz
        var toDoItem = await _repository.GetByIdAsync(id);
        
        if (toDoItem == null)
        {
            return NotFound();
        }

        // Giriş yapmış kullanıcının Id'sini alıyoruz
        var user = await _userManager.GetUserAsync(User);

        if (user == null || toDoItem.UserId != user.Id)
        {
            return Unauthorized(); // Eğer kullanıcı giriş yapmamışsa veya yetkisi yoksa
        }

        // ToDoItem'ı siliyoruz
        await _repository.DeleteAsync(id);

        return RedirectToAction(nameof(Index));
    }
}
