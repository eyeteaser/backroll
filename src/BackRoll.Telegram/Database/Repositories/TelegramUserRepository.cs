using BackRoll.Telegram.Database.Entities;
using LiteDB;

namespace BackRoll.Telegram.Database.Repositories
{
    public class TelegramUserRepository : ITelegramUserRepository
    {
        public const string CollectionName = "TelegramUsers";

        private readonly ILiteCollection<TelegramUserEntity> _collection;

        public TelegramUserRepository(LiteDatabase db)
        {
            _collection = db.GetCollection<TelegramUserEntity>(CollectionName);
        }

        public TelegramUserEntity FindByUserId(long userId)
        {
            return _collection.FindOne(x => x.UserId == userId);
        }

        public void Upsert(TelegramUserEntity user)
        {
            _collection.Upsert(user);
        }
    }
}
