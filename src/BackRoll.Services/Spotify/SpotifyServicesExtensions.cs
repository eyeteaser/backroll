using AutoMapper;
using BackRoll.Services.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BackRoll.Services.Spotify
{
    public static class SpotifyServicesExtensions
    {
        public static IServiceCollection AddSpotify(this IServiceCollection services, IConfiguration configuration)
        {
            var spotifyConfig = new SpotifyConfig();
            configuration.GetSection(SpotifyConfig.CONFIG_SECTION).Bind(spotifyConfig);
            var spotifyClient = SpotifyClientFactory.CreateSpotifyClient(spotifyConfig);
            services.AddSingleton<IStreamingService, SpotifyService>(p => new SpotifyService(spotifyClient, p.GetRequiredService<IMapper>()));

            return services;
        }
    }
}
