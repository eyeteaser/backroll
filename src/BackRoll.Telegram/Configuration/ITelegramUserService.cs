using BackRoll.Services.Models;
using BackRoll.Telegram.Models;
using Telegram.Bot.Types;

namespace BackRoll.Telegram.Configuration
{
    public interface ITelegramUserService
    {
        TelegramUserModel GetUser(User user);

        void SetStreamingService(User user, StreamingService streamingService);

        void SetNotNew(User user);
    }
}
