using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using BackRoll.Services.Abstractions;
using BackRoll.Services.Models;
using SpotifyAPI.Web;

namespace BackRoll.Services.Spotify
{
    public class SpotifyService : IStreamingService
    {
        private readonly SpotifyClient _spotifyClient;
        private readonly IMapper _mapper;

        public SpotifyService(
            SpotifyClient spotifyClient,
            IMapper mapper)
        {
            _spotifyClient = spotifyClient ?? throw new ArgumentNullException(nameof(spotifyClient));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Track> FindTrackAsync(TrackSearchRequest request)
        {
            var searchRequest = new SearchRequest(SearchRequest.Types.Track, request.Query);
            var searchResponse = await _spotifyClient.Search.Item(searchRequest);

            var foundTrack = searchResponse.Tracks.Items.FirstOrDefault();
            var track = Map(foundTrack);

            return track;
        }

        public async Task<Track> GetTrackByUrlAsync(string url)
        {
            Track track = null;
            string id = GetId(url);
            if (!string.IsNullOrEmpty(id))
            {
                var spotifyTrack = await _spotifyClient.Tracks.Get(id);
                track = Map(spotifyTrack);
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
            var match = Regex.Match(url, @"https:\/\/open\.spotify\.com\/track\/(?<id>[a-zA-Z0-9]+)");
            if (match.Success)
            {
                id = match.Groups["id"].Value;
            }

            return id;
        }

        private Track Map(FullTrack spotifyTrack)
        {
            Track track = _mapper.Map<Track>(spotifyTrack);
            if (track != null)
            {
                spotifyTrack.ExternalUrls.TryGetValue("spotify", out string url);
                track.Url = url;
            }

            return track;
        }
    }
}
