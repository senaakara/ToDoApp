using Microsoft.AspNetCore.Identity;

public class User : IdentityUser<int>
{
    public string Role { get; set; }

    // Kullanıcıya ait ToDoItem'lar
    public ICollection<ToDoItem>? ToDoItems { get; set; }
}
