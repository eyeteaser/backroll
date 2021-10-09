using System;
using System.Threading.Tasks;
using BackRoll.Services.Spotify;
using FluentAssertions;
using Tests.BackRoll.Services.TestsInfrastructure;
using Xunit;

namespace Tests.BackRoll.Services.Spotify
{
    [Collection(TestsConstants.MainCollectionName)]
    public class SpotifyClientFactoryTests
    {
        private readonly SpotifyConfig _spotifyConfig;

        public SpotifyClientFactoryTests(ConfigurationFixture configurationFixture)
        {
            _spotifyConfig = configurationFixture.SpotifyConfig;
        }

        [Fact]
        public async Task CreateSpotifyClient()
        {
            // arrange
            // act
            var spotifyClient = SpotifyClientFactory.CreateSpotifyClient(_spotifyConfig);

            // assert
            spotifyClient.Should().NotBeNull();
            Func<Task> act = async () => await spotifyClient.Tracks.Get("1s6ux0lNiTziSrd7iUAADH");
            await act.Should().NotThrowAsync();
        }
    }
}
