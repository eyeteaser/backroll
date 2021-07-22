using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackRoll.Services.Abstractions;
using BackRoll.Services.Exceptions;
using BackRoll.Services.Models;

namespace BackRoll.Services.Services
{
    public class StreamingManager : IStreamingManager
    {
        private readonly IStreamingService[] _streamingServices;

        public StreamingManager(IEnumerable<IStreamingService> streamingServices)
        {
            _streamingServices = streamingServices.ToArray();
        }

        public async Task<Track> FindTrackAsync(string url, StreamingService streamingService)
        {
            var sourceStreamingService = _streamingServices.FirstOrDefault(ss => ss.Match(url));
            if (sourceStreamingService == null)
            {
                throw StreamingServiceNotFoundException.MatchingServiceNotFoundException();
            }

            var targetStreamingService = _streamingServices
                .FirstOrDefault(ss => ss.Name == streamingService);
            if (targetStreamingService == null)
            {
                throw StreamingServiceNotFoundException.ServiceNotFoundException(streamingService);
            }

            var originalTrack = await sourceStreamingService.GetTrackByUrlAsync(url);
            Track track = await targetStreamingService.FindTrackAsync(CreateRequest(originalTrack));
            return track;
        }

        private static TrackSearchRequest CreateRequest(Track track) => new () { Query = $"{track.Name} {string.Join(",", track.Artists.Select(a => a.Name))}" };
    }
}
