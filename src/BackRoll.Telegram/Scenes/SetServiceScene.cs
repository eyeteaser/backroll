using System.Threading.Tasks;
using BackRoll.Services.Models;
using BackRoll.Telegram.Bot;
using BackRoll.Telegram.Configuration;

namespace BackRoll.Telegram.Scenes
{
    public class SetServiceScene : IScene
    {
        public const string CommandPrefix = "/setservice";
        private readonly ITelegramUserService _telegramUserService;
        private readonly ISessionService _sessionService;
        private readonly IStreamingHelper _streamingHelper;

        public SceneType SceneType => SceneType.SetService;

        public SetServiceScene(
            ITelegramUserService telegramUserService,
            ISessionService sessionService,
            IStreamingHelper streamingHelper)
        {
            _telegramUserService = telegramUserService;
            _sessionService = sessionService;
            _streamingHelper = streamingHelper;
        }

        public Task<SceneResponse> ProcessAsync(TelegramMessage message)
        {
            if (message.Text.StartsWith(CommandPrefix)
                && _streamingHelper.TryParseStreamingData(CommandPrefix, message.Text, out StreamingService streamingService))
            {
                _telegramUserService.SetStreamingService(message.From, streamingService);
                var lastRequest = _sessionService.GetAndDeleteLastRequest(message.From.Id);
                var chainedScene = lastRequest != null ? SceneType.Message : SceneType.Undefined;
                message.Text = lastRequest;
                var response = SceneResponse.Ok($"I will remember that you like {_streamingHelper.GetStreamingPrettyName(streamingService)}", chainWith: chainedScene);
                return Task.FromResult(response);
            }

            var markup = _streamingHelper.CreateStreamingButtonsMarkup(CommandPrefix);
            return Task.FromResult(SceneResponse.Ok("What is your favorite streaming service?", markup));
        }
    }
}
