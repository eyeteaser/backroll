namespace BackRoll.Services.Spotify
{
    public class SpotifyConfig
    {
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public SpotifyConfig()
        {
        }

        public SpotifyConfig(string clientId, string clientSecret)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
        }
    }
}
