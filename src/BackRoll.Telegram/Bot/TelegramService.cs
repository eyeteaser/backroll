using System;
using System.Threading;
using System.Threading.Tasks;
using BackRoll.Telegram.Scenes;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BackRoll.Telegram.Bot
{
    public class TelegramService : ITelegramService
    {
        private readonly TelegramBotClient _botClient;
        private readonly IScenesManager _scenesManager;
        private readonly ILogger<TelegramService> _logger;

        public TelegramService(TelegramBotClient botClient, IScenesManager scenesManager, ILogger<TelegramService> logger)
        {
            _botClient = botClient;
            _scenesManager = scenesManager;
            _logger = logger;
        }

        public async Task ProcessUpdateAsync(Update update, CancellationToken stoppingToken = default)
        {
            Chat chat = GetChat(update);
            if (chat != null)
            {
                try
                {
                    if (update.Type != UpdateType.CallbackQuery)
                    {
                        await _botClient.SendChatActionAsync(chat, ChatAction.Typing, stoppingToken);
                    }

                    var response = await _scenesManager.ProcessAsync(GetMessage(update));
                    if (update.Type == UpdateType.CallbackQuery)
                    {
                        await _botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, cancellationToken: stoppingToken);
                    }

                    foreach (var message in response.Messages)
                    {
                        await _botClient.SendTextMessageAsync(chat, message.Text, cancellationToken: stoppingToken, replyMarkup: message.ReplyMarkup);
                    }
                }
                catch (Exception ex)
                {
                    await _botClient.SendTextMessageAsync(chat, "I can't process your request. Please try again later", cancellationToken: stoppingToken);
                    _logger.LogError(ex, ex.Message);
                }
            }
        }

        private static TelegramMessage GetMessage(Update update)
        {
            return update.Type switch
            {
                UpdateType.Message => new TelegramMessage(update.Message.From, update.Message.Text),
                UpdateType.CallbackQuery => new TelegramMessage(update.CallbackQuery.From, update.CallbackQuery.Data),
                _ => null,
            };
        }

        private static Chat GetChat(Update update)
        {
            return update.Type switch
            {
                UpdateType.Message => update.Message.Chat,
                UpdateType.CallbackQuery => update.CallbackQuery.Message.Chat,
                _ => null,
            };
        }
    }
}
