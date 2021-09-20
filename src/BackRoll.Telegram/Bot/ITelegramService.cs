using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace BackRoll.Telegram.Bot
{
    public interface ITelegramService
    {
        Task ProcessUpdateAsync(Update update, CancellationToken stoppingToken = default);
    }
}
