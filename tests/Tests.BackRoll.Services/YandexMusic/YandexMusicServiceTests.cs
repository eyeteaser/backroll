using System.Linq;
using System.Threading.Tasks;
using BackRoll.Services.Models;
using BackRoll.Services.YandexMusic;
using FluentAssertions;
using Tests.BackRoll.Services.TestsInfrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Tests.BackRoll.Services.YandexMusic
{
    [Collection(TestsConstants.MainCollectionName)]
    public class YandexMusicServiceTests : ServicesTestsBase
    {
        public YandexMusicServiceTests(
            ConfigurationFixture configurationFixture,
            ITestOutputHelper testOutputHelper)
            : base(configurationFixture, testOutputHelper)
        {
        }

        [Theory]
        [InlineData("never gonna give you up", "Never Gonna Give You Up", "https://music.yandex.ru/album/67593/track/610031", "Rick Astley")]
        public async Task FindTrackAsync(string query, string expectedName, string expectedUrl, params string[] expectedArtists)
        {
            // arrange
            var trackSearchRequest = new TrackSearchRequest()
            {
                Query = query,
            };
            var yandexMusicService = new YandexMusicService(Mapper);

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
        [InlineData("https://music.yandex.ru/album/67593/track/610031")]
        [InlineData("https://music.yandex.ru/album/67593/track/610031?lang=en", "https://music.yandex.ru/album/67593/track/610031")]
        [InlineData("look at the new episode of rick and morty https://music.yandex.ru/album/67593/track/610031", "https://music.yandex.ru/album/67593/track/610031")]
        [InlineData("look at the new episode of rick and morty https://music.yandex.ru/album/67593/track/610031 it's super cool", "https://music.yandex.ru/album/67593/track/610031")]
        public async Task GetTrackByUrlAsync(string text, string expectedUrl = null)
        {
            // arrange
            if (string.IsNullOrEmpty(expectedUrl))
            {
                expectedUrl = text;
            }

            var yandexMusicService = new YandexMusicService(Mapper);

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
        [InlineData("https://music.yandex.ru/album/67593/track/610031")]
        [InlineData("https://music.yandex.ru/album/67593/track/610031?lang=en")]
        [InlineData("look at the new episode of rick and morty https://music.yandex.ru/album/67593/track/610031")]
        [InlineData("look at the new episode of rick and morty https://music.yandex.ru/album/67593/track/610031 it's super cool")]
        public void Match(string text)
        {
            // arrange
            var yandexMusicService = new YandexMusicService(Mapper);

            // act
            // assert
            yandexMusicService.Match(text).Should().BeTrue();
        }
    }
}
