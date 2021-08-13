using System.Threading.Tasks;
using BackRoll.Services.Abstractions;
using BackRoll.Services.Exceptions;
using BackRoll.Telegram.Configuration;
using BackRoll.Telegram.Exceptions;
using BackRoll.Telegram.Extensions;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace BackRoll.Telegram.Scenes
{
    public class MessageScene : Scene
    {
        private readonly ITelegramUserConfiguration _telegramUserConfiguration;
        private readonly IStreamingManager _streamingManager;
        private readonly ILogger _logger;

        public MessageScene(
            ITelegramUserConfiguration telegramUserConfiguration,
            IStreamingManager streamingManager,
            ILogger<MessageScene> logger)
        {
            _telegramUserConfiguration = telegramUserConfiguration;
            _streamingManager = streamingManager;
            _logger = logger;
        }

        public override async Task<SceneResponse> ProcessAsync(Update update)
        {
            try
            {
                var configuration = _telegramUserConfiguration.GetConfiguration(update.Message.From);
                var track = await _streamingManager.FindTrackAsync(update.Message.Text, configuration.StreamingService);
                var text = track?.Url;
                if (!string.IsNullOrEmpty(text))
                {
                    return SceneResponse.Ok(text);
                }

                return SceneResponse.Fail("Sorry! Not found =(");
            }
            catch (TelegramUserConfigurationNotFoundException)
            {
                return SceneResponse.Fail("Please set your favorite streaming service.\n/setservice");
            }
            catch (StreamingServiceNotFoundException e)
            {
                _logger.LogInformation(e);
                return SceneResponse.Fail("Please input correct link to streaming's track");
            }
            catch (TrackNotFoundException e)
            {
                _logger.LogInformation(e);
                return SceneResponse.Fail("Sorry! Not found =(");
            }
        }
    }
}
