using BackRoll.Services.Models;
using BackRoll.Telegram.Models;
using Telegram.Bot.Types;

namespace BackRoll.Telegram.Configuration
{
    public interface ITelegramUserConfiguration
    {
        TelegramUserConfigurationModel GetConfiguration(User user);

        void SetStreamingService(User user, StreamingService streamingService);
    }
}
