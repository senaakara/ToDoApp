using Microsoft.AspNetCore.Identity;

public class User : IdentityUser<int>
{
   

    // Kullanıcıya ait ToDoItem'lar
    public ICollection<ToDoItem>? ToDoItems { get; set; }
}
