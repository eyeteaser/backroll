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

        public async Task<Track> FindTrackAsync(string url)
        {
            var streamingService = _streamingServices.FirstOrDefault(ss => ss.Match(url));
            if (streamingService == null)
            {
                throw new MatchingStreamingServiceNotFoundException();
            }

            Track track = null;
            if (streamingService != null)
            {
                var originalTrack = await streamingService.GetTrackByUrlAsync(url);
                var service = _streamingServices
                    .FirstOrDefault(ss => ss != streamingService);

                track = await service.FindTrackAsync(CreateRequest(originalTrack));
            }

            return track;
        }

        private static TrackSearchRequest CreateRequest(Track track) => new () { Query = $"{track.Name} {string.Join(",", track.Artists.Select(a => a.Name))}" };
    }
}
