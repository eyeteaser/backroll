using AutoMapper;
using BackRoll.Services.Spotify;
using BackRoll.Services.YandexMusic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BackRoll
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.Configure<BotConfig>(hostContext.Configuration.GetSection(BotConfig.CONFIG_SECTION));
                    services.Configure<SpotifyConfig>(hostContext.Configuration.GetSection(SpotifyConfig.CONFIG_SECTION));

                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.AddProfile<SpotifyProfile>();
                        cfg.AddProfile<YandexMusicProfile>();
                    });
                    services.AddSingleton(x => config.CreateMapper());
                });
    }
}
