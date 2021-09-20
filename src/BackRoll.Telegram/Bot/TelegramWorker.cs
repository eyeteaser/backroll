using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;

namespace BackRoll.Telegram.Bot
{
    public class TelegramWorker : BackgroundService
    {
        private readonly TelegramBotClient _botClient;
        private readonly ITelegramService _telegramService;

        public TelegramWorker(TelegramBotClient botClient, ITelegramService telegramService)
        {
            _botClient = botClient;
            _telegramService = telegramService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var updateReceiver = new QueuedUpdateReceiver(_botClient);

            updateReceiver.StartReceiving(cancellationToken: stoppingToken);

            await foreach (Update update in updateReceiver.YieldUpdatesAsync())
            {
                await _telegramService.ProcessUpdateAsync(update, stoppingToken);
            }
        }
    }
}
