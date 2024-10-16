using Microsoft.AspNetCore.Mvc;
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

    // GET: api/ToDoItem
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ToDoItem>>> GetAll()  // Tüm görevleri listele
    {
        var items = await _repository.GetAllAsync();
        return Ok(items);  // 200 OK döner
    }

    // GET: api/ToDoItem/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ToDoItem>> GetById(int id)  // ID'ye göre görev getir
    {
        var item = await _repository.GetByIdAsync(id);
        if (item == null)
        {
            return NotFound();  // 404 Not Found döner
        }
        return Ok(item);  // 200 OK döner
    }

    // POST: api/ToDoItem
    [HttpPost]
    public async Task<ActionResult<ToDoItem>> Create(ToDoItem item)  // Yeni görev yarat
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);  // 400 Bad Request döner
        }
        await _repository.AddAsync(item);
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);  // 201 Created döner
    }

    // PUT: api/ToDoItem/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, ToDoItem item)  // ID'ye göre güncelleme
    {
        if (id != item.Id)
        {
            return BadRequest();  // 400 Bad Request döner
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);  // 400 Bad Request döner
        }

        await _repository.UpdateAsync(item);
        return NoContent();  // 204 No Content döner
    }

    // DELETE: api/ToDoItem/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)  // ID'ye göre sil
    {
        var item = await _repository.GetByIdAsync(id);
        if (item == null)
        {
            return NotFound();  // 404 Not Found döner
        }

        await _repository.DeleteAsync(id);
        return NoContent();  // 204 No Content döner
    }
}
