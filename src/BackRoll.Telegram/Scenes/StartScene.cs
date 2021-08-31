using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace BackRoll.Telegram.Scenes
{
    public class StartScene : IScene
    {
        public const string Command = "/start";

        public SceneType SceneType => SceneType.Start;

        public Task<SceneResponse> ProcessAsync(TelegramMessage message)
        {
            var button = new KeyboardButton("https://open.spotify.com/track/4cOdK2wGLETKBW3PvgPWqT");
            var markup = new ReplyKeyboardMarkup(new KeyboardButton[] { button }, true, true);
            var response = SceneResponse.Ok(
                "Hi! This bot will help you convert track link from one streaming service to another. Let's see how it works. Please click on the Spotify link below.",
                markup);
            return Task.FromResult(response);
        }
    }
}
