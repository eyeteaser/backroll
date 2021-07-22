using BackRoll.Telegram.Data.Entities;
using LiteDB;

namespace BackRoll.Telegram.Data.Repositories
{
    public class TelegramUserConfigurationRepository : ITelegramUserConfigurationRepository
    {
        private readonly ILiteCollection<TelegramUserConfigurationEntity> _collection;

        public TelegramUserConfigurationRepository(LiteDatabase db)
        {
            _collection = db.GetCollection<TelegramUserConfigurationEntity>("UserConfigurations");
        }

        public TelegramUserConfigurationEntity FindByUserId(long userId)
        {
            return _collection.FindOne(x => x.UserId == userId);
        }

        public void Upsert(TelegramUserConfigurationEntity configuration)
        {
            _collection.Upsert(configuration);
        }
    }
}
