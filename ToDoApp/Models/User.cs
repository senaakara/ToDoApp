public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }

    // Role alanı (Admin mi normal kullanıcı mı)
    public string Role { get; set; }

    // Kullanıcıya ait ToDoItem'lar
    public ICollection<ToDoItem>? ToDoItems { get; set; }
}
