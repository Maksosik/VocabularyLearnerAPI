namespace PropertieForLanguages.DataContext.Sqlite.Models;
public class Word
{
    public int Id { get; set; }
    public int GroupId { get; set; }
    public string Term { get; set; }      // Має збігатися з "Term" у JSON
    public string Language { get; set; }  // Має збігатися з "Language" у JSON
}