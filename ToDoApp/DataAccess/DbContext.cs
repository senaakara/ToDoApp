using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<ToDoItem> ToDoItems { get; set; }  // ToDoItem modeli için DbSet

    protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // ToDoItem ve User arasında bire çok ilişki
    modelBuilder.Entity<ToDoItem>()
        .HasOne(t => t.User)           // Her ToDoItem bir User'a bağlı
        .WithMany(u => u.ToDoItems)    // Bir User birçok ToDoItem'a sahip olabilir
        .HasForeignKey(t => t.UserId)  // ToDoItem, UserId'ye bağlı
        .OnDelete(DeleteBehavior.SetNull);  // Silindiğinde null yap

    base.OnModelCreating(modelBuilder);
}

}
