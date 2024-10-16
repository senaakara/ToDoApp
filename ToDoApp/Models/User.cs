public class User
{
    public int Id { get; set; }  // Kullanıcının benzersiz ID'si
    public string Username { get; set; }  // Kullanıcının adı veya kullanıcı adı
    public string Email { get; set; }  // Kullanıcının e-posta adresi
    public string PasswordHash { get; set; }  // Kullanıcının şifre hash'i (güvenli şifreleme için)

    // İlişkili görevler
    public ICollection<ToDoItem>? ToDoItems { get; set; }
}