using System;
using System.Threading;
using System.Threading.Tasks;
using BackRoll.Services.Abstractions;
using BackRoll.Services.Exceptions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;

namespace BackRoll.Telegram.Bot
{
    public class TelegramWorker : BackgroundService
    {
        private readonly TelegramBotClient _botClient;
        private readonly IStreamingManager _streamingManager;
        private readonly ILogger<TelegramWorker> _logger;

        public TelegramWorker(ILogger<TelegramWorker> logger, TelegramBotClient botClient, IStreamingManager streamingManager)
        {
            _logger = logger;
            _botClient = botClient;
            _streamingManager = streamingManager;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var updateReceiver = new QueuedUpdateReceiver(_botClient);

            updateReceiver.StartReceiving(cancellationToken: stoppingToken);

            await foreach (Update update in updateReceiver.YieldUpdatesAsync())
            {
                if (update.Message is Message message)
                {
                    try
                    {
                        var track = await _streamingManager.FindTrackAsync(message.Text);
                        var text = track?.Url;
                        await _botClient.SendTextMessageAsync(message.Chat, string.IsNullOrEmpty(text) ? "Sorry! Not found =(" : text, cancellationToken: stoppingToken);
                    }
                    catch (MatchingStreamingServiceNotFoundException)
                    {
                        await _botClient.SendTextMessageAsync(message.Chat, $"Please input correct link to streaming's track", cancellationToken: stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, ex.Message);
                    }
                }
            }
        }
    }
}
