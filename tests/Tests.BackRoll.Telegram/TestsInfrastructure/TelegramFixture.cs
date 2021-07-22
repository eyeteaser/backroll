using LiteDB;

namespace Tests.BackRoll.Telegram.TestsInfrastructure
{
    public class TelegramFixture
    {
        public LiteDatabase Db { get; }

        public TelegramFixture()
        {
            Db = new LiteDatabase("telegram.db");
        }
    }
}
