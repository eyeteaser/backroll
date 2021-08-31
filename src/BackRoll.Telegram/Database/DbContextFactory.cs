using System.IO;
using LiteDB;

namespace BackRoll.Telegram.Database
{
    public static class DbContextFactory
    {
        public static LiteDatabase Create(string connectionString)
        {
            string directory = Path.GetDirectoryName(connectionString);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            return new LiteDatabase(connectionString);
        }
    }
}
