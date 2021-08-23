using System.Threading.Tasks;

namespace BackRoll.Telegram.Scenes
{
    public interface IScene
    {
        SceneType SceneType { get; }

        Task<SceneResponse> ProcessAsync(TelegramMessage message);
    }
}
