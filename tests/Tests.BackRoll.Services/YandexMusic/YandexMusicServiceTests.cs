using System.Linq;
using System.Threading.Tasks;
using BackRoll.Services.Models;
using BackRoll.Services.YandexMusic;
using FluentAssertions;
using Tests.BackRoll.Services.TestsInfrastructure;
using Xunit;
using Xunit.Abstractions;
using YandexMusicResolver;

namespace Tests.BackRoll.Services.YandexMusic
{
    [Collection(TestsConstants.MainCollectionName)]
    public class YandexMusicServiceTests : ServicesTestsBase
    {
        private readonly IYandexMusicMainResolver _yandexMusicClient;

        public YandexMusicServiceTests(
            ConfigurationFixture configurationFixture,
            ITestOutputHelper testOutputHelper)
            : base(configurationFixture, testOutputHelper)
        {
            _yandexMusicClient = configurationFixture.YandexMusicClient;
        }

        [Theory]
        [InlineData("never gonna give you up", "Never Gonna Give You Up", "https://music.yandex.ru/track/610031", "Rick Astley")]
        public async Task FindTrackAsync_TrackExists_ShouldFindTrack(string query, string expectedName, string expectedUrl, params string[] expectedArtists)
        {
            // arrange
            var trackSearchRequest = new TrackSearchRequest()
            {
                Query = query,
            };
            var yandexMusicService = new YandexMusicService(_yandexMusicClient, Mapper);

            // act
            var track = await yandexMusicService.FindTrackAsync(trackSearchRequest);

            // assert
            track.Should().NotBeNull();
            track.Name.Should().NotBeNullOrEmpty();
            track.Name.Should().Be(expectedName);
            track.Url.Should().NotBeNullOrEmpty();
            track.Url.Should().Be(expectedUrl);
            track.Artists.Should().NotBeNullOrEmpty();
            track.Artists.Should().OnlyContain(x => !string.IsNullOrEmpty(x.Name));
            track.Artists.Should().Contain(x => expectedArtists.Any(a => a == x.Name));
            LogTrack(track);
        }

        [Theory]
        [InlineData("adgdgdgregdfdfh")]
        public async Task FindTrackAsync_TrackDoesNotExist_ShouldNotFindTrack(string query)
        {
            // arrange
            var trackSearchRequest = new TrackSearchRequest()
            {
                Query = query,
            };
            var yandexMusicService = new YandexMusicService(_yandexMusicClient, Mapper);

            // act
            var track = await yandexMusicService.FindTrackAsync(trackSearchRequest);

            // assert
            track.Should().BeNull();
        }

        [Theory]
        [InlineData("https://music.yandex.ru/album/67593/track/610031", "https://music.yandex.ru/track/610031")]
        [InlineData("https://music.yandex.ru/album/67593/track/610031?lang=en", "https://music.yandex.ru/track/610031")]
        [InlineData("look at the new episode of rick and morty https://music.yandex.ru/album/67593/track/610031", "https://music.yandex.ru/track/610031")]
        [InlineData("look at the new episode of rick and morty https://music.yandex.ru/album/67593/track/610031 it's super cool", "https://music.yandex.ru/track/610031")]
        [InlineData("https://music.yandex.by/album/67593/track/610031", "https://music.yandex.ru/track/610031")]
        [InlineData("https://music.yandex.by/track/610031", "https://music.yandex.ru/track/610031")]
        public async Task GetTrackByUrlAsync_CorrectUrl_ShouldFindTrack(string text, string expectedUrl)
        {
            // arrange
            var yandexMusicService = new YandexMusicService(_yandexMusicClient, Mapper);

            // act
            var track = await yandexMusicService.GetTrackByUrlAsync(text);

            // assert
            track.Should().NotBeNull();
            track.Name.Should().NotBeNullOrEmpty();
            track.Url.Should().NotBeNullOrEmpty();
            track.Url.Should().Be(expectedUrl);
            track.Artists.Should().NotBeNullOrEmpty();
            track.Artists.Should().OnlyContain(x => !string.IsNullOrEmpty(x.Name));
            LogTrack(track);
        }

        [Theory]
        [InlineData("https://music.yandex.ru/album/67593/track/invalid")]
        public async Task GetTrackByUrlAsync_IncorrectUrl_ShouldNotFindTrack(string text)
        {
            // arrange
            var yandexMusicService = new YandexMusicService(_yandexMusicClient, Mapper);

            // act
            var track = await yandexMusicService.GetTrackByUrlAsync(text);

            // assert
            track.Should().BeNull();
        }

        [Theory]
        [InlineData("https://music.yandex.ru/album/67593/track/610031")]
        [InlineData("https://music.yandex.ru/album/67593/track/610031?lang=en")]
        [InlineData("look at the new episode of rick and morty https://music.yandex.ru/album/67593/track/610031")]
        [InlineData("look at the new episode of rick and morty https://music.yandex.ru/album/67593/track/610031 it's super cool")]
        [InlineData("https://music.yandex.by/album/67593/track/610031")]
        [InlineData("https://music.yandex.by/track/610031")]
        public void Match(string text)
        {
            // arrange
            var yandexMusicService = new YandexMusicService(_yandexMusicClient, Mapper);

            // act
            // assert
            yandexMusicService.Match(text).Should().BeTrue();
        }
    }
}
