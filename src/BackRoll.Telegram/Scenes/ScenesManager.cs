using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace BackRoll.Telegram.Scenes
{
    public class ScenesManager : IScenesManager
    {
        private readonly IEnumerable<IScene> _scenes;

        public ScenesManager(IEnumerable<IScene> scenes)
        {
            _scenes = scenes;
        }

        public async Task<SceneResponse> ProcessAsync(Update update)
        {
            var data = GetData(update);
            var sceneType = GetSceneType(data);
            var result = await GetScene(sceneType).ProcessAsync(update);
            if (result.Status == SceneResponseStatus.Redirect)
            {
                result = await GetScene(result.SceneToRedirect).ProcessAsync(update);
            }

            return result;
        }

        private static SceneType GetSceneType(string data)
        {
            if (data.StartsWith(SetServiceScene.CommandPrefix))
            {
                return SceneType.SetService;
            }

            return SceneType.Message;
        }

        private static string GetData(Update update)
        {
            if (update.CallbackQuery != null)
            {
                return update.CallbackQuery.Data;
            }

            if (update.Message != null)
            {
                return update.Message.Text;
            }

            return null;
        }

        private IScene GetScene(SceneType sceneType)
        {
            return _scenes.FirstOrDefault(x => x.SceneType == sceneType);
        }
    }
}
