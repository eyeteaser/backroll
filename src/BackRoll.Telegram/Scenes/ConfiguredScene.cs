using System.Threading.Tasks;

namespace BackRoll.Telegram.Scenes
{
    public class ConfiguredScene : IScene
    {
        public SceneType SceneType => SceneType.Configured;

        public Task<SceneResponse> ProcessAsync(TelegramMessage message)
        {
            var response = SceneResponse.Ok("You are all set! Enjoy using this bot! 🧥👞🎙");
            return Task.FromResult(response);
        }
    }
}
