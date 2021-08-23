using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackRoll.Telegram.Scenes
{
    public class ScenesManager : IScenesManager
    {
        private readonly IEnumerable<IScene> _scenes;

        public ScenesManager(IEnumerable<IScene> scenes)
        {
            _scenes = scenes;
        }

        public async Task<TelegramResponse> ProcessAsync(TelegramMessage message)
        {
            var sceneType = GetSceneType(message.Text);

            var result = new TelegramResponse
            {
                Messages = new List<TelegramResponseMessage>(),
            };

            await ProcessRecursiveAsync(message, sceneType, result);

            return result;
        }

        private static TelegramResponseMessage Map(SceneResponse sceneResponse)
        {
            return new TelegramResponseMessage()
            {
                Text = sceneResponse.Message,
                ReplyMarkup = sceneResponse.ReplyMarkup,
            };
        }

        private static SceneType GetSceneType(string data)
        {
            if (data.StartsWith(SetServiceScene.CommandPrefix))
            {
                return SceneType.SetService;
            }

            return SceneType.Message;
        }

        private IScene GetScene(SceneType sceneType)
        {
            return _scenes.FirstOrDefault(x => x.SceneType == sceneType);
        }

        private async Task ProcessRecursiveAsync(TelegramMessage message, SceneType sceneType, TelegramResponse telegramResponse)
        {
            var sceneResponse = await GetScene(sceneType).ProcessAsync(message);
            if (!string.IsNullOrEmpty(sceneResponse.Message))
            {
                telegramResponse.Messages.Add(Map(sceneResponse));
            }

            if (sceneResponse.ChainWith != SceneType.Undefined)
            {
                await ProcessRecursiveAsync(message, sceneResponse.ChainWith, telegramResponse);
            }
        }
    }
}
