using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BackRoll.Services.Models;
using BackRoll.Telegram.Configuration;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BackRoll.Telegram.Scenes
{
    public class SetServiceScene : IScene
    {
        public const string CommandPrefix = "/setservice";
        private const string _streamingDataRegex = CommandPrefix + @"_(?<service>\w+)";
        private readonly ITelegramUserConfiguration _telegramUserConfiguration;

        public SceneType SceneType => SceneType.SetService;

        public SetServiceScene(ITelegramUserConfiguration telegramUserConfiguration)
        {
            _telegramUserConfiguration = telegramUserConfiguration;
        }

        public Task<SceneResponse> ProcessAsync(Update update)
        {
            if (update.CallbackQuery != null && TryParseStreamingData(update.CallbackQuery.Data, out StreamingService streamingService))
            {
                _telegramUserConfiguration.SetStreamingService(update.CallbackQuery.From, streamingService);
                return Task.FromResult(SceneResponse.Ok($"I will remember that you like {GetStreamingPrettyName(streamingService)}"));
            }

            var buttons = new InlineKeyboardButton[]
            {
                CreateStreamingButton(StreamingService.Spotify),
                CreateStreamingButton(StreamingService.YandexMusic),
            };
            var markup = new InlineKeyboardMarkup(buttons);
            return Task.FromResult(SceneResponse.Ok("What is your favorite streaming service?", markup));
        }

        private static InlineKeyboardButton CreateStreamingButton(StreamingService streamingService)
        {
            return InlineKeyboardButton.WithCallbackData(GetStreamingPrettyName(streamingService), CreateStreamingData(streamingService));
        }

        private static string GetStreamingPrettyName(StreamingService streamingService)
        {
            return streamingService switch
            {
                StreamingService.Spotify => "Spotify",
                StreamingService.YandexMusic => "Yandex Music",
                _ => streamingService.ToString(),
            };
        }

        private static string CreateStreamingData(StreamingService streamingService)
        {
            return $"{CommandPrefix}_{streamingService}";
        }

        private static bool TryParseStreamingData(string data, out StreamingService streamingService)
        {
            var match = Regex.Match(data, _streamingDataRegex);
            if (match.Success)
            {
                string streaming = match.Groups["service"].Value;
                return Enum.TryParse(streaming, out streamingService);
            }

            streamingService = StreamingService.Undefined;
            return false;
        }
    }
}
