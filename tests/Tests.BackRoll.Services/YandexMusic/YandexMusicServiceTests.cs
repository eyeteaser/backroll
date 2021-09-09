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
        private readonly YandexMusicConfig _yandexMusicConfig;

        private readonly YandexMusicService _sut;

        public YandexMusicServiceTests(
            ConfigurationFixture configurationFixture,
            ITestOutputHelper testOutputHelper)
            : base(configurationFixture, testOutputHelper)
        {
            _yandexMusicConfig = configurationFixture.YandexMusicConfig;

            _sut = new YandexMusicService(_yandexMusicConfig, Mapper);
        }

        [Theory]
        [InlineData("Never Gonna Give You Up", "https://music.yandex.ru/album/1910064/track/610031", "Now That's What I Call 30 Years", "Rick Astley")]
        public async Task FindTrackAsync_TrackExists_ShouldFindTrack(string expectedName, string expectedUrl, string expectedAlbum, params string[] expectedArtists)
        {
            // arrange
            var trackSearchRequest = new TrackSearchRequest()
            {
                Track = expectedName,
                Artists = expectedArtists.ToList(),
            };

            // act
            var track = await _sut.FindTrackAsync(trackSearchRequest);

            // assert
            track.Should().NotBeNull();
            track.Name.Should().NotBeNullOrEmpty();
            track.Name.Should().Be(expectedName);
            track.Urls.Should().NotBeNullOrEmpty();
            track.Urls.Should().Contain(expectedUrl);
            track.Artists.Should().NotBeNullOrEmpty();
            track.Artists.Should().OnlyContain(x => !string.IsNullOrEmpty(x.Name));
            track.Artists.Should().Contain(x => expectedArtists.Any(a => a == x.Name));
            track.Album.Should().NotBeNull();
            track.Album.Name.Should().Be(expectedAlbum);
            LogTrack(track);
        }

        [Theory]
        [InlineData("adgdgdgregdfdfh")]
        public async Task FindTrackAsync_TrackDoesNotExist_ShouldNotFindTrack(string trackName)
        {
            // arrange
            var trackSearchRequest = new TrackSearchRequest()
            {
                Track = trackName,
            };

            // act
            var track = await _sut.FindTrackAsync(trackSearchRequest);

            // assert
            track.Should().BeNull();
        }

        [Theory]
        [InlineData("https://music.yandex.ru/album/67593/track/610031", "https://music.yandex.ru/album/67593/track/610031", "Get The Party Started: Essential Pop and Dance Anthems")]
        [InlineData("https://music.yandex.ru/album/67593/track/610031?lang=en", "https://music.yandex.ru/album/67593/track/610031", "Get The Party Started: Essential Pop and Dance Anthems")]
        [InlineData("look at the new episode of rick and morty https://music.yandex.ru/album/67593/track/610031", "https://music.yandex.ru/album/67593/track/610031", "Get The Party Started: Essential Pop and Dance Anthems")]
        [InlineData("look at the new episode of rick and morty https://music.yandex.ru/album/67593/track/610031 it's super cool", "https://music.yandex.ru/album/67593/track/610031", "Get The Party Started: Essential Pop and Dance Anthems")]
        [InlineData("https://music.yandex.by/album/67593/track/610031", "https://music.yandex.ru/album/67593/track/610031", "Get The Party Started: Essential Pop and Dance Anthems")]
        [InlineData("https://music.yandex.ru/album/11575610/track/69278563", "https://music.yandex.ru/album/11575610/track/69278563", "Акустика")]
        public async Task GetTrackByUrlAsync_CorrectUrl_ShouldFindTrack(string text, string expectedUrl, string expectedAlbum = null)
        {
            // arrange
            // act
            var track = await _sut.GetTrackByUrlAsync(text);

            // assert
            track.Should().NotBeNull();
            track.Name.Should().NotBeNullOrEmpty();
            track.Urls.Should().NotBeNullOrEmpty();
            track.Urls.Should().Contain(expectedUrl);
            track.Artists.Should().NotBeNullOrEmpty();
            track.Artists.Should().OnlyContain(x => !string.IsNullOrEmpty(x.Name));
            track.Album.Should().NotBeNull();
            track.Album.Name.Should().Be(expectedAlbum);
            LogTrack(track);
        }

        [Theory]
        [InlineData("https://music.yandex.by/track/610031", "https://music.yandex.ru/track/610031")]
        public async Task GetTrackByUrlAsync_CorrectUrlButNoAlbum_ShouldFindTrackWithoutAlbum(string text, string expectedUrl)
        {
            // arrange
            // act
            var track = await _sut.GetTrackByUrlAsync(text);

            // assert
            track.Should().NotBeNull();
            track.Name.Should().NotBeNullOrEmpty();
            track.Urls.Should().NotBeNullOrEmpty();
            track.Urls.Should().Contain(expectedUrl);
            track.Artists.Should().NotBeNullOrEmpty();
            track.Artists.Should().OnlyContain(x => !string.IsNullOrEmpty(x.Name));
            track.Album.Should().BeNull();
            LogTrack(track);
        }

        [Theory]
        [InlineData("https://music.yandex.ru/album/67593/track/invalid")]
        [InlineData("https://music.yandex.ru/album/1234214214/track/12341242131")]
        public async Task GetTrackByUrlAsync_IncorrectUrl_ShouldNotFindTrack(string text)
        {
            // arrange
            // act
            var track = await _sut.GetTrackByUrlAsync(text);

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
            // act
            // assert
            _sut.Match(text).Should().BeTrue();
        }
    }
}
