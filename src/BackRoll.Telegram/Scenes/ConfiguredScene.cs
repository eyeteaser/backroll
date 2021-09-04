using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace BackRoll.Telegram.Scenes
{
    public class ConfiguredScene : IScene
    {
        public SceneType SceneType => SceneType.Configured;

        public Task<SceneResponse> ProcessAsync(TelegramMessage message)
        {
            var markup = new ReplyKeyboardRemove();
            var response = SceneResponse.Ok("You are all set! Enjoy using this bot! 🧥👞🎙", markup);
            return Task.FromResult(response);
        }
    }
}
