using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using BackRoll.Services.Abstractions;
using BackRoll.Services.Models;
using YandexMusicResolver;
using YandexMusicResolver.AudioItems;

namespace BackRoll.Services.YandexMusic
{
    public class YandexMusicService : IStreamingService
    {
        private readonly IYandexMusicMainResolver _yandexMusicMainResolver;
        private readonly IMapper _mapper;

        public YandexMusicService(IYandexMusicMainResolver yandexMusicMainResolver, IMapper mapper)
        {
            _yandexMusicMainResolver = yandexMusicMainResolver;
            _mapper = mapper;
        }

        public async Task<Track> FindTrackAsync(TrackSearchRequest request)
        {
            var yandexMusicSearchResults = await _yandexMusicMainResolver.SearchResultLoader.LoadSearchResult(YandexSearchType.Track, request.Query);
            var yandexMusicTrack = yandexMusicSearchResults.Tracks?.FirstOrDefault();
            var track = Map(yandexMusicTrack);
            return track;
        }

        public async Task<Track> GetTrackByUrlAsync(string url)
        {
            Track track = null;
            string id = GetId(url);
            if (!string.IsNullOrEmpty(id))
            {
                var yandexMusicTrack = await _yandexMusicMainResolver.TrackLoader.LoadTrack(id);
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

        private Track Map(YandexMusicTrack yandexMusicTrack)
        {
            var track = _mapper.Map<Track>(yandexMusicTrack);

            if (track != null)
            {
                // delete album from url
                // track may be in different albums and album can be not available in the region of user but track may be available
                track.Url = Regex.Replace(track.Url, @"\/album\/\d+", string.Empty);
            }

            return track;
        }
    }
}
