using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
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

        // Admin kullanıcı tüm item'ları görebilir
        if (await _userManager.IsInRoleAsync(user, "Admin"))
        {
            var allToDoItems = await _repository.GetAllAsync();
            return View(allToDoItems);
        }
        else
        {
            // Normal kullanıcı sadece kendi item'larını görür
            var userToDoItems = await _repository.GetToDoItemsByUserIdAsync(user.Id);
            return View(userToDoItems);
        }
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
        toDoItem.UserId = user.Id;

        // Eğer bir son tarih girildiyse, UTC'ye çevir
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

        // Yalnızca admin veya item'in sahibi düzenleme yetkisine sahiptir
        if (!await _userManager.IsInRoleAsync(user, "Admin") && toDoItem.UserId != user.Id)
        {
            return RedirectToAction("AccessDenied", "Account");
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

        if (!await _userManager.IsInRoleAsync(user, "Admin") && toDoItem.UserId != user.Id)
        {
            return RedirectToAction("AccessDenied", "Account");
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

        // Admin değilse ve item kullanıcıya ait değilse, yetkisiz erişim
        if (!await _userManager.IsInRoleAsync(user, "Admin") && toDoItem.UserId != user.Id)
        {
            return Forbid();
        }

        return View(toDoItem);
    }

    // POST: ToDoItem/Delete/5
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

        // Admin değilse ve item kullanıcıya ait değilse, yetkisiz erişim
        if (!await _userManager.IsInRoleAsync(user, "Admin") && toDoItem.UserId != user.Id)
        {
            return Forbid();
        }

        await _repository.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
