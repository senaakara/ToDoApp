using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ToDoItemRepository : IToDoItemRepository
{
    private readonly ApplicationDbContext _context;

    public ToDoItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ToDoItem>> GetAllAsync()
    {
        return await _context.ToDoItems.ToListAsync();
    }

    public async Task<ToDoItem> GetByIdAsync(int id)
    {
        return await _context.ToDoItems.FindAsync(id);
    }

    public async Task AddAsync(ToDoItem entity)
    {
        await _context.ToDoItems.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ToDoItem entity)
    {
        _context.ToDoItems.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.ToDoItems.FindAsync(id);
        if (entity != null)
        {
            _context.ToDoItems.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.ToDoItems.AnyAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<ToDoItem>> GetByUserIdAsync(int userId)
    {
        return await _context.ToDoItems.Where(t => t.UserId == userId).ToListAsync();
    }
    
    public async Task<IEnumerable<ToDoItem>> GetToDoItemsByUserIdAsync(int userId)
    {
        return await _context.ToDoItems.Where(t => t.UserId == userId).ToListAsync();
    }

    public async Task<IEnumerable<ToDoItem>> GetAllAsync(bool includeUser = false)
{
    if (includeUser)
    {
        return await _context.ToDoItems.Include(t => t.User).ToListAsync();
    }
    return await _context.ToDoItems.ToListAsync();
}

public async Task<IEnumerable<ToDoItem>> GetToDoItemsByUserIdAsync(int userId, bool includeUser = false)
{
    if (includeUser)
    {
        return await _context.ToDoItems.Where(t => t.UserId == userId).Include(t => t.User).ToListAsync();
    }
    return await _context.ToDoItems.Where(t => t.UserId == userId).ToListAsync();
}


}
