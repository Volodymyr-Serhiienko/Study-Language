using SQLite;
using System.Diagnostics;

namespace Study_Language
{
    public class DatabaseService
    {
        private readonly SQLiteAsyncConnection _database;

        public DatabaseService(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<WordEntry>().Wait();
            _database.CreateTableAsync<PhraseEntry>().Wait();
        }

        public Task<List<WordEntry>> GetWordsAsync()
        {
            return _database.Table<WordEntry>().ToListAsync();
        }

        public Task<List<PhraseEntry>> GetPhrasesAsync()
        {
            return _database.Table<PhraseEntry>().ToListAsync();
        }

        public Task<int> SaveWordAsync(WordEntry word)
        {
            return _database.InsertAsync(word);
        }

        public Task<int> SavePhraseAsync(PhraseEntry phrase)
        {
            return _database.InsertAsync(phrase);
        }
        
        public Task<int> DeleteWordAsync(string id)
        {
            return _database.Table<WordEntry>().DeleteAsync(x => x.Id == id);
        }

        public Task<int> DeletePhraseAsync(string id)
        {
            return _database.Table<PhraseEntry>().DeleteAsync(x => x.Id == id);
        }

        // Получение слова из БД по имени
        public async Task<WordEntry> GetWordByTextAsync(string word)
        {
            return await _database.Table<WordEntry>()
                .Where(w => w.Word.ToLower() == word.ToLower()) // Игнорирование регистра
                .FirstOrDefaultAsync();
        }

        // Получение предложения из БД по имени
        public async Task<PhraseEntry> GetPhraseByTextAsync(string phrase)
        {
            return await _database.Table<PhraseEntry>()
                .Where(w => w.Phrase.ToLower() == phrase.ToLower()) // Игнорирование регистра
                .FirstOrDefaultAsync();
        }
    }
}
