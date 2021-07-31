using System.Linq;
using System.Text.RegularExpressions;
using BackRoll.Services.Models;

namespace BackRoll.Services.Services
{
    public abstract class BaseStreamingService
    {
        protected virtual string BuildTrackSearchQuery(TrackSearchRequest trackSearchRequest)
        {
            string query = Regex.Replace(trackSearchRequest.Track, @"[^\p{L}\s+]", string.Empty);
            if (trackSearchRequest.Artists != null && trackSearchRequest.Artists.Any())
            {
                query += $" {string.Join(",", trackSearchRequest.Artists)}";
            }

            return query;
        }
    }
}
