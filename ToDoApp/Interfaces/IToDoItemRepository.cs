using System.Collections.Generic;
using System.Threading.Tasks;

public interface IToDoItemRepository : IRepository<ToDoItem>
{
    Task<IEnumerable<ToDoItem>> GetByUserIdAsync(int userId);

    Task<IEnumerable<ToDoItem>> GetToDoItemsByUserIdAsync(int userId);
}
