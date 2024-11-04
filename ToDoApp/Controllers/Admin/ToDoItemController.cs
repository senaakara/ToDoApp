using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[Authorize]
public class ToDoItemController : Controller
{
    private readonly IToDoItemRepository _repository;
    private readonly UserManager<User> _userManager;

    public ToDoItemController(IToDoItemRepository repository, UserManager<User> userManager)
    {
        _repository = repository;
        _userManager = userManager;
    }

     // GET: ToDoItem
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);

        // Admin tüm item'ları görebilir
        if (await _userManager.IsInRoleAsync(user, "Admin"))
        {
            var allToDoItems = await _repository.GetAllAsync();
            return View(allToDoItems);
        }
        else
        {
            // Kullanıcı sadece kendi item'larını görür
            var userToDoItems = await _repository.GetToDoItemsByUserIdAsync(user.Id);
            return View(userToDoItems);
        }
    }

    // GET: ToDoItem/Create
    public async Task<IActionResult> Create()
    {
        ViewData["Title"] = "Yeni ToDo Oluştur";

        var user = await _userManager.GetUserAsync(User);

        // Eğer admin ise kullanıcı listesi ekleyin
        if (await _userManager.IsInRoleAsync(user, "Admin"))
        {
            ViewData["Users"] = _userManager.Users.ToList();
        }

        return View();
    }

    // POST: ToDoItem/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ToDoItem toDoItem)
    {
        var user = await _userManager.GetUserAsync(User);

        // Eğer admin değilse, toDoItem.UserId giriş yapan kullanıcı olarak ayarlanır
        if (!await _userManager.IsInRoleAsync(user, "Admin"))
        {
            toDoItem.UserId = user.Id;
        }

        if (toDoItem.DueDate.HasValue)
        {
            // Bitiş tarihini UTC olarak ayarla
            toDoItem.DueDate = DateTime.SpecifyKind(toDoItem.DueDate.Value, DateTimeKind.Utc);
        }

        if (ModelState.IsValid)
        {
            await _repository.AddAsync(toDoItem);
            return RedirectToAction(nameof(Index));
        }

        return View(toDoItem);
    }

    // GET: ToDoItem/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var toDoItem = await _repository.GetByIdAsync(id);

        if (toDoItem == null)
        {
            return NotFound();
        }

        var user = await _userManager.GetUserAsync(User);

        // Admin için kullanıcı seçimi listesi gönder
        if (await _userManager.IsInRoleAsync(user, "Admin"))
        {
            ViewData["Users"] = _userManager.Users.ToList();
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

        var user = await _userManager.GetUserAsync(User);

        // Eğer admin değilse UserId değişikliğine izin verme
        if (!await _userManager.IsInRoleAsync(user, "Admin"))
        {
            toDoItem.UserId = user.Id; // Sadece kendi UserId'sini korur
        }

        if (toDoItem.DueDate.HasValue)
        {
            toDoItem.DueDate = DateTime.SpecifyKind(toDoItem.DueDate.Value, DateTimeKind.Utc);
        }

        if (ModelState.IsValid)
        {
            await _repository.UpdateAsync(toDoItem);
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
        var user = await _userManager.GetUserAsync(User);

        if (toDoItem == null)
        {
            return NotFound();
        }

        // Sadece adminler diğer kullanıcıların öğelerini silebilir, kullanıcı kendi öğesini silebilir
        if (!await _userManager.IsInRoleAsync(user, "Admin") && toDoItem.UserId != user.Id)
        {
            return Unauthorized();
        }

        return View(toDoItem);
    }

    // POST: ToDoItem/DeleteConfirmed/5
    [HttpPost, ActionName("DeleteConfirmed")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var toDoItem = await _repository.GetByIdAsync(id);
        var user = await _userManager.GetUserAsync(User);

        if (toDoItem == null)
        {
            return NotFound();
        }

        // Sadece adminler diğer kullanıcıların öğelerini silebilir, kullanıcı kendi öğesini silebilir
        if (!await _userManager.IsInRoleAsync(user, "Admin") && toDoItem.UserId != user.Id)
        {
            return Unauthorized();
        }

        await _repository.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
