namespace BackRoll.Services.Models
{
    public class Track
    {
        public string Name { get; set; }

        public Artist[] Artists { get; set; }

        public Album Album { get; set; }

        public string Url { get; set; }
    }
}
