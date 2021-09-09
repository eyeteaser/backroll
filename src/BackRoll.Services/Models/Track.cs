using System;
using System.Collections.Generic;
using System.Linq;

namespace BackRoll.Services.Models
{
    public class Track
    {
        public string Name { get; set; }

        public Artist[] Artists { get; set; }

        public Album Album { get; set; }

        public List<string> Urls { get; set; }

        public override string ToString()
        {
            var artists = string.Join(',', (Artists ?? Array.Empty<Artist>()).Select(x => x.Name));
            var urls = string.Join(',', Urls ?? new List<string>());
            return $"Name: {Name}, Artists: {artists}, Album: {Album?.Name}, Urls: {urls}";
        }
    }
}
