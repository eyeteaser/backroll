using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace BackRoll.Telegram.Scenes
{
    public class ScenesManager : IScenesManager
    {
        private readonly MessageScene _messageScene;
        private readonly SetServiceScene _setServiceScene;

        public ScenesManager(
            MessageScene messageScene,
            SetServiceScene setServiceScene)
        {
            _messageScene = messageScene;
            _setServiceScene = setServiceScene;
        }

        public async Task<SceneResponse> ProcessAsync(Update update)
        {
            string data = GetData(update);

            if (data.StartsWith(SetServiceScene.CommandPrefix))
            {
                return await _setServiceScene.ProcessAsync(update);
            }

            return await _messageScene.ProcessAsync(update);
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
    }
}
