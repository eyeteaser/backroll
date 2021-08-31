using System.Threading.Tasks;
using BackRoll.Services.Abstractions;
using BackRoll.Services.Exceptions;
using BackRoll.Services.Models;
using BackRoll.Telegram.Bot;
using BackRoll.Telegram.Configuration;
using BackRoll.Telegram.Exceptions;
using BackRoll.Telegram.Extensions;
using Microsoft.Extensions.Logging;

namespace BackRoll.Telegram.Scenes
{
    public class MessageScene : IScene
    {
        public const string CommandPrefix = "/message";

        private readonly ITelegramUserService _telegramUserService;
        private readonly IStreamingManager _streamingManager;
        private readonly ISessionService _sessionService;
        private readonly IStreamingHelper _streamingHelper;
        private readonly ILogger _logger;

        public SceneType SceneType => SceneType.Message;

        public MessageScene(
            ITelegramUserService telegramUserService,
            IStreamingManager streamingManager,
            ISessionService sessionService,
            IStreamingHelper streamingHelper,
            ILogger<MessageScene> logger)
        {
            _telegramUserService = telegramUserService;
            _streamingManager = streamingManager;
            _sessionService = sessionService;
            _streamingHelper = streamingHelper;
            _logger = logger;
        }

        public async Task<SceneResponse> ProcessAsync(TelegramMessage message)
        {
            try
            {
                string text;
                if (message.Text.StartsWith(CommandPrefix)
                    && _streamingHelper.TryParseStreamingData(CommandPrefix, message.Text, out StreamingService streamingService))
                {
                    text = _sessionService.GetAndDeleteLastRequest(message.From.Id);
                }
                else
                {
                    var configuration = _telegramUserService.GetUser(message.From);
                    streamingService = configuration.StreamingService;
                    text = message.Text;
                }

                var track = await _streamingManager.FindTrackAsync(text, streamingService);
                var responseText = track?.Url;
                if (!string.IsNullOrEmpty(responseText))
                {
                    return SceneResponse.Ok(responseText);
                }

                return SceneResponse.Fail("Sorry! Not found =(");
            }
            catch (TelegramUserNotFoundException)
            {
                _sessionService.SetLastRequest(message.From.Id, message.Text);
                return SceneResponse.Fail(chainWith: SceneType.SetService);
            }
            catch (StreamingServiceNotFoundException e)
            {
                _logger.LogInformation(e);
                return SceneResponse.Fail("Please input correct link to streaming's track");
            }
            catch (SameStreamingServiceException e)
            {
                _sessionService.SetLastRequest(message.From.Id, message.Text);
                return SceneResponse.Fail(
                    $"This is already {_streamingHelper.GetStreamingPrettyName(e.StreamingService)} link.\nMaybe you want to convert it to other platform link?",
                    _streamingHelper.CreateStreamingButtonsMarkup(CommandPrefix, e.StreamingService));
            }
            catch (TrackNotFoundException e)
            {
                _logger.LogInformation(e);
                return SceneResponse.Fail("Sorry! Not found =(");
            }
        }
    }
}
