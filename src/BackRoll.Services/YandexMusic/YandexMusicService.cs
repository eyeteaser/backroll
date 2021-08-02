using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using BackRoll.Services.Abstractions;
using BackRoll.Services.Models;
using Yandex.Music.Api;
using Yandex.Music.Api.Common;
using Yandex.Music.Api.Models.Search.Track;
using Yandex.Music.Api.Models.Track;

namespace BackRoll.Services.YandexMusic
{
    public class YandexMusicService : IStreamingService
    {
        private readonly YandexMusicApi _yandexMusicClient;
        private readonly AuthStorage _authStorage;
        private readonly IMapper _mapper;

        public StreamingService Name => StreamingService.YandexMusic;

        public YandexMusicService(YandexMusicConfig yandexMusicConfig, IMapper mapper)
        {
            (_yandexMusicClient, _authStorage) = GetYandexMusicClient(yandexMusicConfig);
            _mapper = mapper;
        }

        public async Task<Track> FindTrackAsync(TrackSearchRequest request)
        {
            var yandexMusicSearchResults = await _yandexMusicClient.Search.TrackAsync(_authStorage, request.Query);
            var yandexMusicTrack = yandexMusicSearchResults.Result.Tracks?.Results.FirstOrDefault();
            if (yandexMusicTrack != null)
            {
                var track = Map(yandexMusicTrack);
                return track;
            }

            return null;
        }

        public async Task<Track> GetTrackByUrlAsync(string url)
        {
            Track track = null;
            string id = GetId(url);
            if (!string.IsNullOrEmpty(id))
            {
                var yandexMusicTrack = (await _yandexMusicClient.Track.GetAsync(_authStorage, id)).Result.FirstOrDefault();
                track = Map(yandexMusicTrack);
            }

            return track;
        }

        public bool Match(string url)
        {
            return !string.IsNullOrEmpty(GetId(url));
        }

        private static string GetId(string url)
        {
            string id = null;
            var match = Regex.Match(url, @"https:\/\/music\.yandex\.\w+(\/album\/\d+)?\/track\/(?<id>\d+)");
            if (match.Success)
            {
                id = match.Groups["id"].Value;
            }

            return id;
        }

        private static (YandexMusicApi, AuthStorage) GetYandexMusicClient(YandexMusicConfig config)
        {
            var authStorage = new AuthStorage();
            if (config.UseProxy)
            {
                authStorage.Context.WebProxy = new WebProxy(new Uri($"http://{config.ProxyHost}:{config.ProxyPort}"))
                {
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(
                        userName: config.ProxyUserName,
                        password: config.ProxyPassword),
                };
            }

            return (new YandexMusicApi(), authStorage);
        }

        private static void SetTrackUrl(Track track, string trackId)
        {
            if (track != null)
            {
                // set track url ignoring album
                // track may be in different albums and album can be not available in the region of user but track may be available
                track.Url = $"https://music.yandex.ru/track/{trackId}";
            }
        }

        private Track Map(YTrack yandexMusicTrack)
        {
            var track = _mapper.Map<Track>(yandexMusicTrack);
            SetTrackUrl(track, yandexMusicTrack.Id);
            return track;
        }

        private Track Map(YSearchTrackModel yandexMusicTrack)
        {
            var track = _mapper.Map<Track>(yandexMusicTrack);
            SetTrackUrl(track, yandexMusicTrack.Id);
            return track;
        }
    }
}
