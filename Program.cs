using Microsoft.EntityFrameworkCore;
using PropertieForLanguages.DataContext.Sqlite.Data;
using PropertieForLanguages.DataContext.Sqlite.Models; // Додай це для класу Word
using System.Text.Json; // Необхідно для роботи з JSON

var builder = WebApplication.CreateBuilder(args);

// 1. Додаємо підтримку контролерів
builder.Services.AddControllers();

// 2. Реєструємо наш контекст бази даних
builder.Services.AddDbContext<VocabularyContext>(options =>
    options.UseSqlite("Data Source=vocabulary.db"));

// 3. Налаштовуємо CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// --- початок блоку ініціалізації бд, якщо цього не було ще ---
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<VocabularyContext>();
    context.Database.EnsureCreated();

    if (!context.Words.Any())
    {
        try
        {
            var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "words.json");
            if (File.Exists(jsonPath))
            {
                var jsonData = File.ReadAllText(jsonPath);

                // КЛЮЧОВИЙ МОМЕНТ: Додаємо налаштування ігнорування регістру
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var words = JsonSerializer.Deserialize<List<Word>>(jsonData, options);

                if (words != null && words.Any())
                {
                    // Фільтруємо на всяк випадок порожні записи перед збереженням
                    var validWords = words.Where(w => !string.IsNullOrEmpty(w.Term)).ToList();

                    context.Words.AddRange(validWords);
                    context.SaveChanges();
                    Console.WriteLine($"Успішно завантажено {validWords.Count} слів у базу!");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка імпорту: {ex.Message}");
        }
    }
}
// --- кінець ініціалізації бд ---

// 4. Вмикаємо CORS та маршрутизацію
app.UseCors();
app.MapControllers();

app.Run();