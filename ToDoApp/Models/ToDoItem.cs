public class ToDoItem
{
    public int Id { get; set; }  // Her bir göreve özgü ID
    public string Title { get; set; }  // Görevin başlığı
    public string Description { get; set; }  // Görevin açıklaması (opsiyonel olabilir)
    public DateTime? DueDate { get; set; }  // Görevin son teslim tarihi
    public bool IsCompleted { get; set; }  // Görevin tamamlanma durumu
    
    public int UserId { get; set; }  // Foreign Key
    public User? User { get; set; }  // Navigasyon özelliği


    
}