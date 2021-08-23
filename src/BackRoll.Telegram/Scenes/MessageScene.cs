using System.Threading.Tasks;
using BackRoll.Services.Abstractions;
using BackRoll.Services.Exceptions;
using BackRoll.Telegram.Configuration;
using BackRoll.Telegram.Exceptions;
using BackRoll.Telegram.Extensions;
using Microsoft.Extensions.Logging;

namespace BackRoll.Telegram.Scenes
{
    public class MessageScene : IScene
    {
        private readonly ITelegramUserConfiguration _telegramUserConfiguration;
        private readonly IStreamingManager _streamingManager;
        private readonly ISessionService _sessionService;
        private readonly ILogger _logger;

        public SceneType SceneType => SceneType.Message;

        public MessageScene(
            ITelegramUserConfiguration telegramUserConfiguration,
            IStreamingManager streamingManager,
            ISessionService sessionService,
            ILogger<MessageScene> logger)
        {
            _telegramUserConfiguration = telegramUserConfiguration;
            _streamingManager = streamingManager;
            _sessionService = sessionService;
            _logger = logger;
        }

        public async Task<SceneResponse> ProcessAsync(TelegramMessage message)
        {
            try
            {
                var configuration = _telegramUserConfiguration.GetConfiguration(message.From);
                var track = await _streamingManager.FindTrackAsync(message.Text, configuration.StreamingService);
                var text = track?.Url;
                if (!string.IsNullOrEmpty(text))
                {
                    return SceneResponse.Ok(text);
                }

                return SceneResponse.Fail("Sorry! Not found =(");
            }
            catch (TelegramUserConfigurationNotFoundException)
            {
                _sessionService.SetLastRequest(message.From.Id, message.Text);
                return SceneResponse.Fail(chainWith: SceneType.SetService);
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
