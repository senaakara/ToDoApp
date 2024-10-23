using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;  // Kullanıcı bilgilerini almak için gerekli

[Authorize]  // Giriş yapılmasını zorunlu hale getiriyoruz
public class ToDoItemController : Controller
{
    private readonly IToDoItemRepository _repository;
    private readonly UserManager<User> _userManager;  // Kullanıcının bilgilerini almak için

    public ToDoItemController(IToDoItemRepository repository, UserManager<User> userManager)
    {
        _repository = repository;
        _userManager = userManager;
    }

    // GET: ToDoItem (Sadece giriş yapan kullanıcının to-do'larını getiriyoruz)
    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);  // Giriş yapan kullanıcının UserId'sini alıyoruz
        var toDoItems = await _repository.GetByUserIdAsync(int.Parse(userId));  // Kullanıcının to-do'larını getiriyoruz
        return View(toDoItems);
    }

    // GET: ToDoItem/Create (Yeni to-do oluşturma formu)
    public IActionResult Create()
    {
        return View();
    }

    // POST: ToDoItem/Create (Yeni to-do oluştur)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ToDoItem toDoItem)
    {
        if (ModelState.IsValid)
        {
            var userId = _userManager.GetUserId(User);  // Giriş yapan kullanıcının UserId'sini alıyoruz
            toDoItem.UserId = int.Parse(userId);  // To-do'ya ait kullanıcı ID'yi ekliyoruz

            // DateTime'i UTC'ye çeviriyoruz
            if (toDoItem.DueDate.HasValue)
            {
                toDoItem.DueDate = DateTime.SpecifyKind(toDoItem.DueDate.Value, DateTimeKind.Utc);
            }

            await _repository.AddAsync(toDoItem);
            return RedirectToAction(nameof(Index));
        }

        return View(toDoItem);
    }

    // GET: ToDoItem/Edit/5 (To-do düzenleme sayfası)
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var toDoItem = await _repository.GetByIdAsync(id.Value);
        if (toDoItem == null || toDoItem.UserId != int.Parse(_userManager.GetUserId(User)))  // Sadece kendi to-do'sunu düzenlemesine izin veriyoruz
        {
            return Unauthorized();  // Yetkisiz işlem
        }

        return View(toDoItem);
    }

    // POST: ToDoItem/Edit/5 (To-do düzenleme işlemi)
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
            try
            {
                // DateTime'i UTC'ye çeviriyoruz
                if (toDoItem.DueDate.HasValue)
                {
                    toDoItem.DueDate = DateTime.SpecifyKind(toDoItem.DueDate.Value, DateTimeKind.Utc);
                }

                var userId = _userManager.GetUserId(User);  // Giriş yapan kullanıcının UserId'sini alıyoruz
                if (toDoItem.UserId != int.Parse(userId))  // Kullanıcının sadece kendi to-do'sunu düzenlemesini sağlıyoruz
                {
                    return Unauthorized();
                }

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

        var toDoItem = await _repository.GetByIdAsync(id.Value);
        if (toDoItem == null || toDoItem.UserId != int.Parse(_userManager.GetUserId(User)))  // Kullanıcının sadece kendi to-do'sunu silmesine izin veriyoruz
        {
            return Unauthorized();
        }

        return View(toDoItem);
    }

    // POST: ToDoItem/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var toDoItem = await _repository.GetByIdAsync(id);
        if (toDoItem.UserId != int.Parse(_userManager.GetUserId(User)))  // Sadece kendi to-do'sunu silebilmesini sağlıyoruz
        {
            return Unauthorized();
        }

        await _repository.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
