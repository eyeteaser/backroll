using BackRoll.Telegram.Data.Entities;

namespace BackRoll.Telegram.Data.Repositories
{
    public interface ITelegramUserConfigurationRepository
    {
        TelegramUserConfigurationEntity FindByUserId(long userId);

        void Upsert(TelegramUserConfigurationEntity configuration);
    }
}
