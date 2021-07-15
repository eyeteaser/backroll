using System;
using AutoMapper;
using BackRoll.Services.Services;
using BackRoll.Services.Spotify;
using BackRoll.Services.YandexMusic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpotifyAPI.Web;

namespace Tests.BackRoll.Services.TestsInfrastructure
{
    public class ConfigurationFixture
    {
        public IServiceProvider Services { get; }

        public IMapper Mapper { get; }

        public SpotifyConfig SpotifyConfig { get; }

        public SpotifyClient SpotifyClient { get; }

        public ConfigurationFixture()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.testing.json")
                .AddUserSecrets(GetType().Assembly)
                .Build();

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<SpotifyProfile>();
                cfg.AddProfile<YandexMusicProfile>();
            });
            Mapper = mockMapper.CreateMapper();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(Mapper);
            serviceCollection
                .AddSpotify(configuration)
                .AddYandexMusic()
                .AddServices();

            Services = serviceCollection.BuildServiceProvider();

            SpotifyConfig = new SpotifyConfig();
            configuration.GetSection("Spotify").Bind(SpotifyConfig);

            SpotifyClient = SpotifyClientFactory.CreateSpotifyClient(SpotifyConfig);
        }
    }
}
