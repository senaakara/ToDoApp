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
        IEnumerable<ToDoItem> toDoItems;

        if (await _userManager.IsInRoleAsync(user, "Admin"))
        {
            toDoItems = await _repository.GetAllAsync();
        }
        else
        {
            toDoItems = await _repository.GetToDoItemsByUserIdAsync(user.Id);
        }

        return View(toDoItems);
    }

    // GET: ToDoItem/Create
    public async Task<IActionResult> Create()
    {
        // Admin kullanıcıya kullanıcı seçimi için liste gönderiyoruz
        var user = await _userManager.GetUserAsync(User);
        if (await _userManager.IsInRoleAsync(user, "Admin"))
        {
            ViewData["Users"] = _userManager.Users.ToList();
        }
        return View();
    }

    // POST: ToDoItem/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ToDoItem toDoItem, int? selectedUserId)
    {
        var user = await _userManager.GetUserAsync(User);

        // Kullanıcı bir admin değilse, kendi UserId'sini ekler
        if (!await _userManager.IsInRoleAsync(user, "Admin"))
        {
            toDoItem.UserId = user.Id;
        }
        else if (selectedUserId.HasValue)
        {
            toDoItem.UserId = selectedUserId.Value;
        }

        if (toDoItem.DueDate.HasValue)
        {
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
    public async Task<IActionResult> Edit(int? id)
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

       
        

        // Admin için kullanıcı seçimi ve kullanıcı bilgilerini göstermek
        if (await _userManager.IsInRoleAsync(user, "Admin"))
        {
            ViewData["Users"] = _userManager.Users.ToList();
            ViewData["OwnerUserName"] = (await _userManager.FindByIdAsync(toDoItem.UserId.ToString()))?.UserName;
        }

        return View(toDoItem);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ToDoItem toDoItem)
    {
        if (id != toDoItem.Id)
        {
            return NotFound();
        }

        // Giriş yapan kullanıcıyı alıyoruz
        var user = await _userManager.GetUserAsync(User);

        // UserId'nin mevcut kullanıcı olduğundan emin olun
        toDoItem.UserId = user.Id;

        if (toDoItem.DueDate.HasValue)
        {
            // Bitiş tarihini UTC olarak ayarla
            toDoItem.DueDate = DateTime.SpecifyKind(toDoItem.DueDate.Value, DateTimeKind.Utc);
        }

        if (ModelState.IsValid)
        {
            try
            {
                await _repository.UpdateAsync(toDoItem);
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "Veritabanına kaydedilirken bir hata oluştu. Lütfen tekrar deneyin.");
                return View(toDoItem);
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
