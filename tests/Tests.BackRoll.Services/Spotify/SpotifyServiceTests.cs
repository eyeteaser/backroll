using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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
    public class SpotifyServiceTests
    {
        private readonly SpotifyClient _spotifyClient;
        private readonly IMapper _mapper;
        private readonly ITestOutputHelper _testOutputHelper;

        public SpotifyServiceTests(
            ConfigurationFixture configurationFixture,
            ITestOutputHelper testOutputHelper)
        {
            _spotifyClient = configurationFixture.SpotifyClient;
            _mapper = configurationFixture.Mapper;
            _testOutputHelper = testOutputHelper;
        }

        [Theory]
        [InlineData("уходи anacondaz", "Уходи", "Anacondaz")]
        [InlineData("don't you worry child", "Don't You Worry Child - Radio Edit", "Swedish House Mafia", "John Martin")]
        public async Task FindTrack_TrackExists_ShouldReturnTrack(
            string query, string expectedName, params string[] expectedArtists)
        {
            // arrange
            var spotifyService = new SpotifyService(_spotifyClient, _mapper);
            var trackSearchRequest = new TrackSearchRequest()
            {
                Query = query,
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
            _testOutputHelper.WriteLine($"Track: {track.Name}");
            _testOutputHelper.WriteLine($"Artists: {string.Join(',', track.Artists.Select(x => x.Name))}");
            _testOutputHelper.WriteLine($"Url: {track.Url}");
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

            var spotifyService = new SpotifyService(_spotifyClient, _mapper);

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
            var spotifyService = new SpotifyService(_spotifyClient, _mapper);

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
            var spotifyService = new SpotifyService(_spotifyClient, _mapper);

            // act
            var result = spotifyService.Match(url);

            // assert
            result.Should().BeFalse();
        }
    }
}
