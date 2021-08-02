using BackRoll.Services.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BackRoll.Services.YandexMusic
{
    public static class YandexMusicServicesExtensions
    {
        public static IServiceCollection AddYandexMusic(this IServiceCollection services, IConfiguration configuration)
        {
            var yandexMusicConfig = new YandexMusicConfig();
            configuration.GetSection(YandexMusicConfig.CONFIG_SECTION).Bind(yandexMusicConfig);
            services.AddSingleton(yandexMusicConfig);

            services.AddSingleton<IStreamingService, YandexMusicService>();

            return services;
        }
    }
}
