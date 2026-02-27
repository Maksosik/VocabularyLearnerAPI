using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertieForLanguages.DataContext.Sqlite.Data;
using PropertieForLanguages.DataContext.Sqlite.Models;

namespace PropertieForLanguages.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WordsController : ControllerBase
{
    private readonly VocabularyContext _db;

    public WordsController(VocabularyContext db)
    {
        _db = db;
    }

    // 1. Отримати випадкове слово з усіма перекладами
    [HttpGet("random")]
    public async Task<ActionResult<IEnumerable<Word>>> GetRandomPair(string fromLang = "en", string toLang = "uk")
    {
        int maxGroupId = await _db.Words.MaxAsync(w => (int?)w.GroupId) ?? 0;
        if (maxGroupId == 0) return NotFound("База даних порожня");

        var random = new Random();
        int randomGroupId = random.Next(1, maxGroupId + 1);

        // 2. Фільтруємо базу так, щоб дістати лише два рядки: мову "З" та мову "НА"
        // Це працює дуже швидко завдяки індексу на GroupId
        var wordPair = await _db.Words
            .Where(w => w.GroupId == randomGroupId && (w.Language == fromLang || w.Language == toLang))
            .ToListAsync();

        if (wordPair.Count < 2)
        {
            // Якщо раптом для цієї групи немає однієї з мов, спробуємо ще раз (рекурсія) 
            // або повернемо помилку
            return NotFound("Пара мов для цієї групи не знайдена");
        }

        return Ok(wordPair);
    }

    //// 2. Додати нове слово (те, що ми обговорювали про цикл роботи API)
    //[HttpPost]
    //public async Task<ActionResult<Word>> PostWord(Word word)
    //{
    //    // Якщо це нове слово, нам треба знайти наступний вільний GroupId
    //    if (word.GroupId == 0)
    //    {
    //        int maxGroupId = await _db.Words.MaxAsync(w => (int?)w.GroupId) ?? 0;
    //        word.GroupId = maxGroupId + 1;
    //    }

    //    _db.Words.Add(word);
    //    await _db.SaveChangesAsync();

    //    return CreatedAtAction(nameof(GetRandomSet), new { id = word.Id }, word);
    //}
}