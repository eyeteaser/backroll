using System.Collections.Generic;

namespace BackRoll.Services.Models
{
    public class TrackSearchRequest
    {
        public string Track { get; set; }

        public List<string> Artists { get; set; }

        public string Album { get; set; }
    }
}
