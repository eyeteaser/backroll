using BackRoll.Services.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace BackRoll.Services.YandexMusic
{
    public static class YandexMusicServicesExtensions
    {
        public static IServiceCollection AddYandexMusic(this IServiceCollection services)
        {
            services.AddSingleton<IStreamingService, YandexMusicService>();

            return services;
        }
    }
}
