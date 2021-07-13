using AutoMapper;
using BackRoll.Services.Spotify;
using BackRoll.Services.YandexMusic;
using Microsoft.Extensions.Configuration;
using SpotifyAPI.Web;

namespace Tests.BackRoll.Services.TestsInfrastructure
{
    public class ConfigurationFixture
    {
        public IMapper Mapper { get; }

        public SpotifyConfig SpotifyConfig { get; }

        public SpotifyClient SpotifyClient { get; }

        public ConfigurationFixture()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.testing.json")
                .AddUserSecrets(GetType().Assembly)
                .Build();

            SpotifyConfig = new SpotifyConfig();
            configuration.GetSection("Spotify").Bind(SpotifyConfig);

            SpotifyClient = SpotifyClientFactory.CreateSpotifyClient(SpotifyConfig);

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<SpotifyProfile>();
                cfg.AddProfile<YandexMusicProfile>();
            });
            Mapper = mockMapper.CreateMapper();
        }
    }
}
