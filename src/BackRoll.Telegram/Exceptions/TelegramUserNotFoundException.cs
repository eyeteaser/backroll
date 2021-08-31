using BackRoll.Services.Exceptions;
using Telegram.Bot.Types;

namespace BackRoll.Telegram.Exceptions
{
    public class TelegramUserNotFoundException : BackRollException
    {
        public TelegramUserNotFoundException(User user)
            : base(0, $"Configuration for \"{user?.Id}\" user not found")
        {
        }
    }
}
