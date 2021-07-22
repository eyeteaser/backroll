using System.Threading.Tasks;
using BackRoll.Services.Models;

namespace BackRoll.Services.Abstractions
{
    public interface IStreamingService
    {
        StreamingService Name { get; }

        Task<Track> FindTrackAsync(TrackSearchRequest request);

        Task<Track> GetTrackByUrlAsync(string url);

        bool Match(string url);
    }
}
