using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BackRoll.Services.Abstractions;
using BackRoll.Services.Models;
using BackRoll.Telegram.Configuration;
using Telegram.Bot.Types.ReplyMarkups;

namespace BackRoll.Telegram.Scenes
{
    public class SetServiceScene : IScene
    {
        public const string CommandPrefix = "/setservice";
        private const string _streamingDataRegex = CommandPrefix + @"_(?<service>\w+)";
        private readonly ITelegramUserConfiguration _telegramUserConfiguration;
        private readonly ISessionService _sessionService;

        public SceneType SceneType => SceneType.SetService;

        public SetServiceScene(
            ITelegramUserConfiguration telegramUserConfiguration,
            ISessionService sessionService)
        {
            _telegramUserConfiguration = telegramUserConfiguration;
            _sessionService = sessionService;
        }

        public Task<SceneResponse> ProcessAsync(TelegramMessage message)
        {
            if (message.Text.StartsWith(CommandPrefix) && TryParseStreamingData(message.Text, out StreamingService streamingService))
            {
                _telegramUserConfiguration.SetStreamingService(message.From, streamingService);
                var lastRequest = _sessionService.GetAndDeleteLastRequest(message.From.Id);
                var chainedScene = lastRequest != null ? SceneType.Message : SceneType.Undefined;
                message.Text = lastRequest;
                var response = SceneResponse.Ok($"I will remember that you like {GetStreamingPrettyName(streamingService)}", chainWith: chainedScene);
                return Task.FromResult(response);
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
