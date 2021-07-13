using AutoMapper;
using BackRoll.Services.Abstractions;
using BackRoll.Services.Models;
using BackRoll.Services.Spotify;
using BackRoll.Services.YandexMusic;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;

namespace BackRoll
{
    public class Worker : BackgroundService
    {
        private readonly IStreamingService[] _streamingServices;
        private readonly TelegramBotClient _botClient;
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger, IOptions<BotConfig> botConfig, IOptions<SpotifyConfig> spotifyConfig, IMapper mapper)
        {
            _logger = logger;
            _botClient = new TelegramBotClient(botConfig.Value.Secret);
            var spotifyClient = SpotifyClientFactory.CreateSpotifyClient(spotifyConfig.Value);
            var spotifyService = new SpotifyService(spotifyClient, mapper);

            _streamingServices = new IStreamingService[2];
            _streamingServices[0] = spotifyService;
            _streamingServices[1] = new YandexMusicService(mapper);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var updateReceiver = new QueuedUpdateReceiver(_botClient);

            updateReceiver.StartReceiving(cancellationToken: stoppingToken);

            await foreach (Update update in updateReceiver.YieldUpdatesAsync())
            {
                if (update.Message is Message message)
                {
                    try
                    {
                        var streamingService = _streamingServices.FirstOrDefault(ss => ss.Match(message.Text));

                        if (streamingService != null)
                        {
                            var track = await streamingService.GetTrackByUrlAsync(message.Text);
                            var tasks = _streamingServices.Where(ss => ss != streamingService).Select(ss => ss.FindTrackAsync(CreateRequest(track))).ToArray();
                            await Task.WhenAll(tasks);
                            var text = string.Join("\n", tasks.Select(t => t.Result?.Url));
                            await _botClient.SendTextMessageAsync(message.Chat, string.IsNullOrEmpty(text) ? "Sorry! Not found =(" : text);
                        }
                        else
                        {
                            await _botClient.SendTextMessageAsync(message.Chat, $"Please input correct link to streaming's track");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, ex.Message);
                    }
                }
            }
        }

        private static TrackSearchRequest CreateRequest(Track track) => new TrackSearchRequest() { Query = $"{track.Name} {string.Join(",", track.Artists.Select(a => a.Name))}" };
    }
}
