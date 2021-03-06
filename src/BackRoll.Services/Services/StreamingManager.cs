using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackRoll.Services.Abstractions;
using BackRoll.Services.Comparers;
using BackRoll.Services.Exceptions;
using BackRoll.Services.Models;

namespace BackRoll.Services.Services
{
    public class StreamingManager : IStreamingManager
    {
        private readonly IStreamingService[] _streamingServices;
        private readonly IEqualityComparer<Track> _trackEqualityComparer;

        public StreamingManager(IEnumerable<IStreamingService> streamingServices)
        {
            _streamingServices = streamingServices.ToArray();
            _trackEqualityComparer = new FuzzyTrackEqualityComparer();
        }

        public async Task<Track> FindTrackAsync(string url, StreamingService streamingService)
        {
            var sourceStreamingService = _streamingServices.FirstOrDefault(ss => ss.Match(url));
            if (sourceStreamingService == null)
            {
                throw StreamingServiceNotFoundException.MatchingServiceNotFound(url);
            }

            var targetStreamingService = _streamingServices
                .FirstOrDefault(ss => ss.Name == streamingService);
            if (targetStreamingService == null)
            {
                throw StreamingServiceNotFoundException.ServiceNotFound(streamingService);
            }

            if (sourceStreamingService == targetStreamingService)
            {
                throw SameStreamingServiceException.Create(streamingService);
            }

            var originalTrack = await sourceStreamingService.GetTrackByUrlAsync(url);
            if (originalTrack == null)
            {
                throw TrackNotFoundException.TrackNotFoundByUrl(sourceStreamingService.Name, url);
            }

            var request = CreateRequest(originalTrack);
            var track = await targetStreamingService.FindTrackAsync(request);
            if (track == null || !track.Urls.Any())
            {
                throw TrackNotFoundException.TrackNotFoundByQuery(targetStreamingService.Name, request);
            }

            if (!_trackEqualityComparer.Equals(originalTrack, track))
            {
                throw WrongTrackFoundException.Create(originalTrack, track);
            }

            return track;
        }

        private static TrackSearchRequest CreateRequest(Track track) => new ()
        {
            Track = track.Name,
            Artists = track.Artists.Select(a => a.Name).ToList(),
            Album = track.Album?.Name,
        };
    }
}
