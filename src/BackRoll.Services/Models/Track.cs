using System;
using System.Linq;

namespace BackRoll.Services.Models
{
    public class Track
    {
        public string Name { get; set; }

        public Artist[] Artists { get; set; }

        public Album Album { get; set; }

        public string Url { get; set; }

        public override string ToString()
        {
            var artists = string.Join(',', (Artists ?? Array.Empty<Artist>()).Select(x => x.Name));
            return $"Name: {Name}, Artists: {artists}, Album: {Album?.Name}, URL: {Url}";
        }
    }
}
