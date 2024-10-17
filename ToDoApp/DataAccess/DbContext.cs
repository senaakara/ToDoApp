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
        // İlişkiyi yapılandırın: Bir User'ın birden fazla ToDoItem'ı olabilir
        modelBuilder.Entity<ToDoItem>()
            .HasOne(t => t.User)           // Her ToDoItem bir User'a ait
            .WithMany(u => u.ToDoItems)    // Bir User birden fazla ToDoItem'a sahip olabilir
            .HasForeignKey(t => t.UserId); // ToDoItem, UserId ile User'a bağlıdır

        base.OnModelCreating(modelBuilder);
    }
}
