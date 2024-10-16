using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserRepository _repository;

    public UserController(IUserRepository repository)
    {
        _repository = repository;
    }

    // GET: api/User
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()  // Tüm kullanıcıları listele
    {
        var users = await _repository.GetAllAsync();
        return Ok(users);  // 200 OK döner
    }

    // GET: api/User/5
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUserById(int id)  // ID'ye göre kullanıcı getir
    {
        var user = await _repository.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound();  // 404 Not Found döner
        }
        return Ok(user);  // 200 OK döner
    }

    // POST: api/User
    [HttpPost]
    public async Task<ActionResult<User>> CreateUser(User user)  // Yeni kullanıcı yarat
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);  // 400 Bad Request döner
        }

        await _repository.AddAsync(user);
        return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);  // 201 Created döner
    }

    // PUT: api/User/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, User user)  // ID'ye göre kullanıcı güncelleme
    {
        if (id != user.Id)
        {
            return BadRequest();  // 400 Bad Request döner
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);  // 400 Bad Request döner
        }

        await _repository.UpdateAsync(user);
        return NoContent();  // 204 No Content döner
    }

    // DELETE: api/User/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)  // ID'ye göre kullanıcı sil
    {
        var user = await _repository.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound();  // 404 Not Found döner
        }

        await _repository.DeleteAsync(id);
        return NoContent();  // 204 No Content döner
    }
}
