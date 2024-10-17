using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

[ApiController]
[Route("api/[controller]")]
public class ToDoItemController : ControllerBase
{
    private readonly IToDoItemRepository _repository;

    public ToDoItemController(IToDoItemRepository repository)
    {
        _repository = repository;
    }

    // Kullanıcı sadece kendi to-do'larını görür
    [HttpGet("{userId}")]
    public async Task<ActionResult<IEnumerable<ToDoItem>>> GetUserToDos(int userId)
    {
        var items = await _repository.GetAllAsync();
        var userItems = items.Where(t => t.UserId == userId).ToList();
        return Ok(userItems);
    }

    // Belirli bir to-do item getir
    [HttpGet("todo/{id}")]
    public async Task<ActionResult<ToDoItem>> GetToDoItemById(int id)
    {
        var item = await _repository.GetByIdAsync(id);
        if (item == null)
        {
            return NotFound();
        }
        return Ok(item);
    }

    // Yeni to-do item ekle
    [HttpPost]
    public async Task<ActionResult<ToDoItem>> CreateToDoItem(ToDoItem item)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);  // Hatalıysa BadRequest döner
        }

        await _repository.AddAsync(item);
        return CreatedAtAction(nameof(GetToDoItemById), new { id = item.Id }, item);
    }

    // To-do item güncelle
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateToDoItem(int id, ToDoItem item)
    {
        if (id != item.Id)
        {
            return BadRequest();
        }

        await _repository.UpdateAsync(item);
        return NoContent();
    }

    // To-do item sil
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteToDoItem(int id)
    {
        var item = await _repository.GetByIdAsync(id);
        if (item == null)
        {
            return NotFound();
        }

        await _repository.DeleteAsync(id);
        return NoContent();
    }
}
