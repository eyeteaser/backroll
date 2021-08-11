using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using BackRoll.Services.Abstractions;
using BackRoll.Services.Models;
using BackRoll.Services.Services;
using SpotifyAPI.Web;

namespace BackRoll.Services.Spotify
{
    public class SpotifyService : BaseStreamingService, IStreamingService
    {
        private readonly SpotifyClient _spotifyClient;
        private readonly IMapper _mapper;

        public override StreamingService Name => StreamingService.Spotify;

        public override string TrackUrlRegex => @"https:\/\/open\.spotify\.com\/track\/(?<trackid>[a-zA-Z0-9]+)";

        public SpotifyService(
            SpotifyClient spotifyClient,
            IMapper mapper)
        {
            _spotifyClient = spotifyClient ?? throw new ArgumentNullException(nameof(spotifyClient));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        protected override async Task<Track> GetTrackByUrlInternalAsync(TrackUrlInfo trackUrlInfo)
        {
            var spotifyTrack = await _spotifyClient.Tracks.Get(trackUrlInfo.TrackId);
            return Map(spotifyTrack);
        }

        protected override async Task<Track> FindTrackInternalAsync(TrackSearchRequest request, string query)
        {
            var searchRequest = new SearchRequest(SearchRequest.Types.Track, query);
            var searchResponse = await _spotifyClient.Search.Item(searchRequest);

            var foundTrack = searchResponse.Tracks.Items.FirstOrDefault(x => x.Album.Name == request.Album);
            if (foundTrack == null)
            {
                foundTrack = searchResponse.Tracks.Items.FirstOrDefault();
            }

            var track = Map(foundTrack);

            return track;
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
