using SQLite;
using SmartCubeMobile.MockData;

namespace SmartCubeMobile.Services
{
    public class StoredTransaction
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public string AccountName { get; set; }
        public string Type { get; set; }
        public string Provider { get; set; }
        public string UniqueKey { get; set; }
    }

    public static class TransactionDatabase
    {
        private static SQLiteConnection _db;
        private static readonly string _dbPath = Path.Combine(
            SmartCubeMobile.Services.AppPaths.Local,
            "SmartCube", "transactions.db");

        static TransactionDatabase()
        {
            var dir = Path.GetDirectoryName(_dbPath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            _db = new SQLiteConnection(_dbPath);
            _db.CreateTable<StoredTransaction>();
        }

        public static int SaveTransactions(List<MockTransaction> transactions, string provider = "TrueLayer")
        {
            int added = 0;
            foreach (var t in transactions)
            {
                var key = $"{t.Date:yyyyMMdd}|{t.Description}|{t.Amount}|{t.AccountName}";

                var exists = _db.Table<StoredTransaction>()
                    .FirstOrDefault(s => s.UniqueKey == key);
                if (exists != null) continue;

                _db.Insert(new StoredTransaction
                {
                    Date = t.Date,
                    Description = t.Description,
                    Category = t.Category,
                    Amount = t.Amount,
                    Balance = t.Balance,
                    AccountName = t.AccountName,
                    Type = t.Type,
                    Provider = provider,
                    UniqueKey = key,
                });
                added++;
            }
            return added;
        }

        public static List<MockTransaction> GetAllTransactions()
        {
            return _db.Table<StoredTransaction>()
                .OrderByDescending(t => t.Date)
                .ToList()
                .Select(t => new MockTransaction
                {
                    Date = t.Date,
                    Description = t.Description,
                    Category = t.Category,
                    Amount = t.Amount,
                    Balance = t.Balance,
                    AccountName = t.AccountName,
                    Type = t.Type,
                })
                .ToList();
        }

        public static int Count => _db.Table<StoredTransaction>().Count();

        public static DateTime? OldestDate()
        {
            var oldest = _db.Table<StoredTransaction>()
                .OrderBy(t => t.Date)
                .FirstOrDefault();
            return oldest?.Date;
        }

        public static DateTime? NewestDate()
        {
            var newest = _db.Table<StoredTransaction>()
                .OrderByDescending(t => t.Date)
                .FirstOrDefault();
            return newest?.Date;
        }

        public static void Clear()
        {
            _db.DeleteAll<StoredTransaction>();
        }
    }
}
