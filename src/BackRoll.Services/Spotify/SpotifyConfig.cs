namespace BackRoll.Services.Spotify
{
    public class SpotifyConfig
    {
        public const string CONFIG_SECTION = "Spotify";

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }
    }
}
