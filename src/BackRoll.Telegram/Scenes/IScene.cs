using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace BackRoll.Telegram.Scenes
{
    public interface IScene
    {
        SceneType SceneType { get; }

        Task<SceneResponse> ProcessAsync(Update update);
    }
}
