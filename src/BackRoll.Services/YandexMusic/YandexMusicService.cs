using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using BackRoll.Services.Abstractions;
using BackRoll.Services.Models;
using YandexMusicResolver;
using YandexMusicResolver.Config;

namespace BackRoll.Services.YandexMusic
{
    public class YandexMusicService : IStreamingService
    {
        private readonly YandexMusicMainResolver _yandexMusicMainResolver;
        private readonly IMapper _mapper;

        public YandexMusicService(IMapper mapper)
        {
            var yandexConfig = new EmptyYandexConfig();
            _yandexMusicMainResolver = new YandexMusicMainResolver(yandexConfig);
            _mapper = mapper;
        }

        public async Task<Track> FindTrackAsync(TrackSearchRequest request)
        {
            var yandexMusicSearchResults = await _yandexMusicMainResolver.SearchResultLoader.LoadSearchResult(YandexSearchType.Track, request.Query);
            var yandexMusicTrack = yandexMusicSearchResults.Tracks.FirstOrDefault();
            var track = _mapper.Map<Track>(yandexMusicTrack);
            return track;
        }

        public async Task<Track> GetTrackByUrlAsync(string url)
        {
            Track track = null;
            string id = GetId(url);
            if (!string.IsNullOrEmpty(id))
            {
                var yandexMusicTrack = await _yandexMusicMainResolver.TrackLoader.LoadTrack(id);
                track = _mapper.Map<Track>(yandexMusicTrack);
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
            var match = Regex.Match(url, @"https:\/\/music\.yandex\.ru\/album\/\d+\/track\/(?<id>\d+)");
            if (match.Success)
            {
                id = match.Groups["id"].Value;
            }

            return id;
        }
    }
}
