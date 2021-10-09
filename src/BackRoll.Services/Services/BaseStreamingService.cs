using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BackRoll.Services.Abstractions;
using BackRoll.Services.Models;

namespace BackRoll.Services.Services
{
    public abstract class BaseStreamingService : IStreamingService
    {
        public abstract StreamingService Name { get; }

        public abstract string TrackUrlRegex { get; }

        public async Task<Track> FindTrackAsync(TrackSearchRequest request)
        {
            string query = BuildTrackSearchQuery(request);
            var track = await FindTrackInternalAsync(request, query);
            PostProcessTrack(track);
            return track;
        }

        public async Task<Track> GetTrackByUrlAsync(string url)
        {
            Track track = null;
            var trackUrlInfo = ParseTrackUrl(url);
            if (trackUrlInfo != null)
            {
                track = await GetTrackByUrlInternalAsync(trackUrlInfo);
            }

            PostProcessTrack(track);
            return track;
        }

        public bool Match(string url)
        {
            return ParseTrackUrl(url) != null;
        }

        protected abstract Task<Track> FindTrackInternalAsync(TrackSearchRequest request, string query);

        protected abstract Task<Track> GetTrackByUrlInternalAsync(TrackUrlInfo trackUrlInfo);

        protected TrackUrlInfo ParseTrackUrl(string url)
        {
            TrackUrlInfo trackUrlInfo = null;
            var match = Regex.Match(url, TrackUrlRegex);
            if (match.Success && match.Groups["trackid"].Success)
            {
                trackUrlInfo = new TrackUrlInfo()
                {
                    TrackId = match.Groups["trackid"].Value,
                    AlbumId = match.Groups["albumid"].Value,
                };
            }

            return trackUrlInfo;
        }

        /// <summary>
        /// Replaces incorrect symbols.
        /// </summary>
        /// <param name="track">Track to process.</param>
        protected virtual void PostProcessTrack(Track track)
        {
            if (track != null)
            {
                track.Name = track.Name.Replace("ё", "ё");
            }
        }

        protected virtual string BuildTrackSearchQuery(TrackSearchRequest trackSearchRequest)
        {
            // remove special characters that not part of the word
            string query = string.Join(' ', trackSearchRequest.Track
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Where(x => Regex.IsMatch(x, @"\p{L}+[^\p{L}]?\p{L}+")));
            if (trackSearchRequest.Artists != null && trackSearchRequest.Artists.Any())
            {
                query += $" {string.Join(",", trackSearchRequest.Artists)}";
            }

            return query;
        }

        protected class TrackUrlInfo
        {
            public string TrackId { get; set; }

            public string AlbumId { get; set; }
        }
    }
}
