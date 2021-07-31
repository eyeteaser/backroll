using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackRoll.Services.Models;
using BackRoll.Services.Spotify;
using FluentAssertions;
using SpotifyAPI.Web;
using Tests.BackRoll.Services.TestsInfrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Tests.BackRoll.Services.Spotify
{
    [Collection(TestsConstants.MainCollectionName)]
    public class SpotifyServiceTests : ServicesTestsBase
    {
        private readonly SpotifyClient _spotifyClient;

        public SpotifyServiceTests(
            ConfigurationFixture configurationFixture,
            ITestOutputHelper testOutputHelper)
            : base(configurationFixture, testOutputHelper)
        {
            _spotifyClient = configurationFixture.SpotifyClient;
        }

        [Theory]
        [InlineData("уходи", "anacondaz", "Уходи", "Перезвони мне +79995771202", "Anacondaz")]
        [InlineData("don't you worry child", "swedish house mafia", "Don't You Worry Child - Radio Edit", "Don't You Worry Child", "Swedish House Mafia", "John Martin")]
        [InlineData("ядреность образ жизни", "нейромонах", "Ядрёность - Образ Жизни (Акустическая Версия)", "Акустика", "Neuromonakh Feofan")]
        public async Task FindTrack_TrackExists_ShouldReturnTrack(
            string trackName, string artist, string expectedName, string expectedAlbum, params string[] expectedArtists)
        {
            // arrange
            var spotifyService = new SpotifyService(_spotifyClient, Mapper);
            var trackSearchRequest = new TrackSearchRequest()
            {
                Track = trackName,
                Artists = new List<string>() { artist },
                Album = expectedAlbum,
            };

            // act
            var track = await spotifyService.FindTrackAsync(trackSearchRequest);

            // assert
            track.Should().NotBeNull();
            track.Name.Should().NotBeNullOrEmpty();
            track.Name.Should().Be(expectedName);
            track.Artists.Should().NotBeNullOrEmpty();
            track.Artists.Should().Match(x => x.All(a => expectedArtists.Contains(a.Name)));
            track.Url.Should().NotBeNullOrEmpty();
            track.Url.Should().StartWith("https://open.spotify.com/track/");
            track.Album.Should().NotBeNull();
            track.Album.Name.Should().Be(expectedAlbum);
            LogTrack(track);
        }

        [Theory]
        [InlineData("https://open.spotify.com/track/4cOdK2wGLETKBW3PvgPWqT")]
        [InlineData("https://open.spotify.com/track/4cOdK2wGLETKBW3PvgPWqT?si=91b97dc299684fad", "https://open.spotify.com/track/4cOdK2wGLETKBW3PvgPWqT")]
        [InlineData("Послушай новый трек https://open.spotify.com/track/4cOdK2wGLETKBW3PvgPWqT", "https://open.spotify.com/track/4cOdK2wGLETKBW3PvgPWqT")]
        [InlineData("Послушай новый трек https://open.spotify.com/track/4cOdK2wGLETKBW3PvgPWqT это огонь", "https://open.spotify.com/track/4cOdK2wGLETKBW3PvgPWqT")]
        [InlineData("🎙 https://open.spotify.com/track/4cOdK2wGLETKBW3PvgPWqT", "https://open.spotify.com/track/4cOdK2wGLETKBW3PvgPWqT")]
        [InlineData("всеhttps://open.spotify.com/track/4cOdK2wGLETKBW3PvgPWqTслиплось", "https://open.spotify.com/track/4cOdK2wGLETKBW3PvgPWqT")]
        [InlineData("а если https://open.spotify.com/track/4cOdK2wGLETKBW3PvgPWqT\n в несколько строк?", "https://open.spotify.com/track/4cOdK2wGLETKBW3PvgPWqT")]
        public async Task GetTrack_CorrectUrl_ShouldFindTrack(string url, string expectedUrl = null)
        {
            // arrange
            if (expectedUrl == null)
            {
                expectedUrl = url;
            }

            var spotifyService = new SpotifyService(_spotifyClient, Mapper);

            // act
            var track = await spotifyService.GetTrackByUrlAsync(url);

            // assert
            track.Name.Should().NotBeNullOrEmpty();
            track.Artists.Should().NotBeNullOrEmpty();
            track.Url.Should().Be(expectedUrl);
        }

        [Theory]
        [InlineData("https://open.spotify.com/track/4cOdK2wGLETKBW3PvgPWqT")]
        [InlineData("https://open.spotify.com/track/4cOdK2wGLETKBW3PvgPWqT?si=91b97dc299684fad")]
        [InlineData("Послушай новый трек https://open.spotify.com/track/4cOdK2wGLETKBW3PvgPWqT")]
        [InlineData("Послушай новый трек https://open.spotify.com/track/4cOdK2wGLETKBW3PvgPWqT это огонь")]
        public void Match_HasSpotifyUrl_ShouldMatch(string url)
        {
            // arrange
            var spotifyService = new SpotifyService(_spotifyClient, Mapper);

            // act
            var result = spotifyService.Match(url);

            // assert
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("https://music.yandex.ru/album/3475523/track/28973341?lang=en")]
        [InlineData("https://open.spotify.com/track/")]
        public void Match_DoNotHaveCorrectSpotifyUrl_ShouldNotMatch(string url)
        {
            // arrange
            var spotifyService = new SpotifyService(_spotifyClient, Mapper);

            // act
            var result = spotifyService.Match(url);

            // assert
            result.Should().BeFalse();
        }
    }
}
