using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace BackRoll.Telegram.Scenes
{
    public abstract class Scene
    {
        public abstract Task<SceneResponse> ProcessAsync(Update update);
    }
}
