using Microsoft.EntityFrameworkCore;
using PropertieForLanguages.DataContext.Sqlite.Models;

namespace PropertieForLanguages.DataContext.Sqlite.Data;

public class VocabularyContext : DbContext
{
    public VocabularyContext(DbContextOptions<VocabularyContext> options) : base(options) { }

    public DbSet<Word> Words => Set<Word>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Налаштовуємо індекси для швидкого пошуку
        modelBuilder.Entity<Word>()
            .HasIndex(w => new { w.Term, w.Language });

        modelBuilder.Entity<Word>()
            .HasIndex(w => w.GroupId);

    }
}//винести логіку наповнення бд в окремий клас-ініціалізатор з program.cs