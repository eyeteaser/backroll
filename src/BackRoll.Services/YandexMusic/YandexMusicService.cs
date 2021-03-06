using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using BackRoll.Services.Abstractions;
using BackRoll.Services.Comparers;
using BackRoll.Services.Models;
using BackRoll.Services.Services;
using Yandex.Music.Api;
using Yandex.Music.Api.Common;
using Yandex.Music.Api.Models.Search.Album;
using Yandex.Music.Api.Models.Search.Track;
using Yandex.Music.Api.Models.Track;

namespace BackRoll.Services.YandexMusic
{
    public class YandexMusicService : BaseStreamingService, IStreamingService
    {
        private readonly YandexMusicApi _yandexMusicClient;
        private readonly AuthStorage _authStorage;
        private readonly IMapper _mapper;
        private readonly IEqualityComparer<string> _fuzzyStringEqualityComparer;
        private readonly string _trackBaseUrl;

        public override StreamingService Name => StreamingService.YandexMusic;

        public override string TrackUrlRegex => @"https:\/\/music\.yandex\.\w+(?:\/album\/(?<albumid>\d+))?\/track\/(?<trackid>\d+)";

        public YandexMusicService(YandexMusicConfig yandexMusicConfig, IMapper mapper)
        {
            (_yandexMusicClient, _authStorage) = GetYandexMusicClient(yandexMusicConfig);
            _mapper = mapper;
            _fuzzyStringEqualityComparer = new FuzzyStringEqualityComparer();
            _trackBaseUrl = "https://music.yandex.ru";
        }

        protected override async Task<Track> GetTrackByUrlInternalAsync(TrackUrlInfo trackUrlInfo)
        {
            try
            {
                var yandexMusicTrack = (await _yandexMusicClient.Track.GetAsync(_authStorage, trackUrlInfo.TrackId)).Result.FirstOrDefault();
                return Map(yandexMusicTrack, trackUrlInfo.AlbumId);
            }
            catch (WebException)
            {
                return null;
            }
        }

        protected override async Task<Track> FindTrackInternalAsync(TrackSearchRequest request, string query)
        {
            var yandexMusicSearchResults = await _yandexMusicClient.Search.TrackAsync(_authStorage, query);
            if (yandexMusicSearchResults.Result.Tracks != null && yandexMusicSearchResults.Result.Tracks.Results.Any())
            {
                // we will always return first track in case if we didn't it by album
                var firstTrack = yandexMusicSearchResults.Result.Tracks.Results.First();
                var trackAlbum = new Tuple<YSearchTrackModel, YSearchAlbumModel>(firstTrack, firstTrack.Albums.FirstOrDefault());

                var tracks = yandexMusicSearchResults.Result.Tracks.Results
                    .Where(x => _fuzzyStringEqualityComparer.Equals(request.Track, x.Title))
                    .ToArray();

                foreach (var yandexMusicTrack in tracks)
                {
                    var album = yandexMusicTrack.Albums.FirstOrDefault(x => x.Title == request.Album);
                    if (album != null)
                    {
                        trackAlbum = new Tuple<YSearchTrackModel, YSearchAlbumModel>(yandexMusicTrack, album);
                        break;
                    }
                }

                var track = Map(trackAlbum.Item1, trackAlbum.Item2);
                return track;
            }

            return null;
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

        private void SetTrackUrl(Track track, string trackId, string albumId)
        {
            if (track != null)
            {
                track.Urls = new List<string>()
                {
                    GetTrackUrl(trackId, albumId),
                    GetTrackUrl(trackId),
                };
            }
        }

        private string GetTrackUrl(string trackId, string albumId = null)
        {
            string trackUrl = _trackBaseUrl;
            if (!string.IsNullOrEmpty(albumId))
            {
                trackUrl += $"/album/{albumId}";
            }

            trackUrl += $"/track/{trackId}";
            return trackUrl;
        }

        private Track Map(YTrack yandexMusicTrack, string albumId)
        {
            var track = _mapper.Map<Track>(yandexMusicTrack);
            if (yandexMusicTrack != null)
            {
                var album = yandexMusicTrack.Albums.FirstOrDefault(x => x.Id == albumId);
                track.Album = _mapper.Map<Album>(album);
                SetTrackUrl(track, yandexMusicTrack.Id, albumId);
            }

            return track;
        }

        private Track Map(YSearchTrackModel yandexMusicTrack, YSearchAlbumModel album)
        {
            var track = _mapper.Map<Track>(yandexMusicTrack);
            if (yandexMusicTrack != null)
            {
                track.Album = _mapper.Map<Album>(album);
                SetTrackUrl(track, yandexMusicTrack.Id, album?.Id);
            }

            return track;
        }
    }
}
