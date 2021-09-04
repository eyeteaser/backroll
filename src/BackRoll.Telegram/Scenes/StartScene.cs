using System.Threading.Tasks;
using BackRoll.Telegram.Configuration;
using Telegram.Bot.Types.ReplyMarkups;

namespace BackRoll.Telegram.Scenes
{
    public class StartScene : IScene
    {
        public const string Command = "/start";
        private readonly ISessionService _sessionService;

        public SceneType SceneType => SceneType.Start;

        public StartScene(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public Task<SceneResponse> ProcessAsync(TelegramMessage message)
        {
            _sessionService.SetUnprocessedScene(message.From.Id, SceneType);

            var button = new KeyboardButton("https://open.spotify.com/track/4cOdK2wGLETKBW3PvgPWqT");
            var markup = new ReplyKeyboardMarkup(new KeyboardButton[] { button }, true, true);
            var response = SceneResponse.Ok(
                "Hi! This bot will help you convert track link from one streaming service to another. Let's see how it works. Please click on the Spotify link below.",
                markup);
            return Task.FromResult(response);
        }
    }
}
