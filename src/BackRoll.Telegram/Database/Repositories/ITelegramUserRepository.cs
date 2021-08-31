using BackRoll.Telegram.Database.Entities;

namespace BackRoll.Telegram.Database.Repositories
{
    public interface ITelegramUserRepository
    {
        TelegramUserEntity FindByUserId(long userId);

        void Upsert(TelegramUserEntity user);
    }
}
