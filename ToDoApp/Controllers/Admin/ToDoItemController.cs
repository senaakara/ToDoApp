using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;


public class ToDoItemController : Controller
{
    private readonly IToDoItemRepository _repository;
    private readonly IUserRepository _userRepository;

    public ToDoItemController(IToDoItemRepository repository, IUserRepository userRepository)
    {
        _repository = repository;
        _userRepository = userRepository;
    }

    // GET: ToDoItem
    public async Task<IActionResult> Index()
    {
        var toDoItems = await _repository.GetAllAsync();
        return View(toDoItems);
    }

    // GET: ToDoItem/Create
    public async Task<IActionResult> Create()
    {
        // Kullanıcı listesini ViewBag ile gönderiyoruz
        ViewBag.Users = await _userRepository.GetAllAsync();
        return View();
    }


    // POST: ToDoItem/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ToDoItem toDoItem)
    {
        if (ModelState.IsValid)
        {
            // DateTime'i UTC'ye çeviriyoruz
            if (toDoItem.DueDate.HasValue)
            {
                toDoItem.DueDate = DateTime.SpecifyKind(toDoItem.DueDate.Value, DateTimeKind.Utc);
            }

            await _repository.AddAsync(toDoItem);
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Users = _userRepository.GetAllAsync(); // Hata durumunda kullanıcı listesi tekrar yüklenmeli
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

    // Kullanıcı listesini ViewBag ile gönderiyoruz
    ViewBag.Users = await _userRepository.GetAllAsync();
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
            try
            {
                // DateTime'i UTC'ye çeviriyoruz
                if (toDoItem.DueDate.HasValue)
                {
                    toDoItem.DueDate = DateTime.SpecifyKind(toDoItem.DueDate.Value, DateTimeKind.Utc);
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

        ViewBag.Users = _userRepository.GetAllAsync(); // Hata durumunda kullanıcı listesi tekrar yüklenmeli
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
        if (toDoItem == null)
        {
            return NotFound();
        }

        return View(toDoItem);
    }

    // POST: ToDoItem/Delete/5
    [HttpPost, ActionName("DeleteConfirmed")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _repository.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
