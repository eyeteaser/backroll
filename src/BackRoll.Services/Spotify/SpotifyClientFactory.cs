using System;
using SpotifyAPI.Web;

namespace BackRoll.Services.Spotify
{
    public static class SpotifyClientFactory
    {
        public static SpotifyClient CreateSpotifyClient(SpotifyConfig config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var spotifyClientConfig = SpotifyClientConfig
              .CreateDefault()
              .WithAuthenticator(new ClientCredentialsAuthenticator(config.ClientId, config.ClientSecret));

            var spotify = new SpotifyClient(spotifyClientConfig);
            return spotify;
        }
    }
}
