using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace BackRoll.Telegram.Scenes
{
    public interface IScenesManager
    {
        Task<SceneResponse> ProcessAsync(Update update);
    }
}
