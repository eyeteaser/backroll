using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BackRoll.Services.Abstractions;
using BackRoll.Services.Exceptions;
using BackRoll.Services.Models;
using BackRoll.Services.Services;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Tests.BackRoll.Services.TestsInfrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Tests.BackRoll.Services.Services
{
    [Collection(TestsConstants.MainCollectionName)]
    public class StreamingManagerTests : ServicesTestsBase
    {
        private readonly IServiceProvider _services;

        private readonly StreamingManager _sut;

        public StreamingManagerTests(ConfigurationFixture configurationFixture, ITestOutputHelper testOutputHelper)
            : base(configurationFixture, testOutputHelper)
        {
            _services = configurationFixture.Services;

            _sut = new StreamingManager(_services.GetService<IEnumerable<IStreamingService>>());
        }

        [Theory]
        [InlineData("i don't have url")]
        public void FindTrack_InvalidUrl_ShouldThrowException(string text)
        {
            // arrange
            // act
            Func<Task> act = () => _sut.FindTrackAsync(text, StreamingService.Undefined);

            // assert
            act.Should().Throw<StreamingServiceNotFoundException>()
                .Which.ErrorCode.Should().Be(ErrorCode.MatchingServiceNotFound);
        }

        [Fact]
        public void FindTrack_UnknownStreamingService_ShouldThrowException()
        {
            // arrange
            // act
            Func<Task> act = () => _sut.FindTrackAsync("https://open.spotify.com/track/4cOdK2wGLETKBW3PvgPWqT", StreamingService.Undefined);

            // assert
            act.Should().Throw<StreamingServiceNotFoundException>()
                .Which.ErrorCode.Should().Be(ErrorCode.ServiceNotFound);
        }

        [Theory]
        [InlineData("https://open.spotify.com/track/4cOdK2wGLETKBW3PvgPWqT", "https://music.yandex.ru/album/1910064/track/610031", StreamingService.YandexMusic)]
        [InlineData("https://music.yandex.ru/album/11575610/track/69278563", "https://open.spotify.com/track/3ZFNg261EvTB9sBElOpoWj", StreamingService.Spotify)]
        [InlineData("https://open.spotify.com/track/3ZFNg261EvTB9sBElOpoWj", "https://music.yandex.ru/album/2455685/track/21458627", StreamingService.YandexMusic)]
        public async Task FindTrack_CorrectUrl_ShouldReturnTrack(string source, string target, StreamingService targetName)
        {
            // arrange
            // act
            var track = await _sut.FindTrackAsync(source, targetName);

            // assert
            track.Should().NotBeNull();
            track.Urls.Should().Contain(target);
        }

        [Theory]
        [InlineData("https://music.yandex.ru/album/1234214214/track/12341242131")]
        public void FindTrack_CorrectUrlFormatButInvalidTrackOrAlbumId_ShouldThrowException(string url)
        {
            // arrange
            // act
            Func<Task> act = () => _sut.FindTrackAsync(url, StreamingService.Spotify);

            // assert
            act.Should().Throw<TrackNotFoundException>()
                .Which.ErrorCode.Should().Be(ErrorCode.TrackNotFoundByUrl);
        }

        [Theory]
        [InlineData("https://open.spotify.com/track/5l2aRFeetTo8rXRcas5U9L")]
        public void FindTrack_TrackExistsInOneStreamingPlatformButNotInOther_ShouldThrowException(string source)
        {
            // arrange
            // act
            Func<Task> act = () => _sut.FindTrackAsync(source, StreamingService.YandexMusic);

            // assert
            act.Should().Throw<WrongTrackFoundException>()
                .Which.ErrorCode.Should().Be(ErrorCode.WrongTrackFound);
        }
    }
}
