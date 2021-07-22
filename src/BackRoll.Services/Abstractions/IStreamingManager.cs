using System.Threading.Tasks;
using BackRoll.Services.Models;

namespace BackRoll.Services.Abstractions
{
    public interface IStreamingManager
    {
        Task<Track> FindTrackAsync(string url, StreamingService streamingService);
    }
}
