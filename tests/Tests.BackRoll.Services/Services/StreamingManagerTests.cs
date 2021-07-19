using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BackRoll.Services.Abstractions;
using BackRoll.Services.Exceptions;
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

        public StreamingManagerTests(ConfigurationFixture configurationFixture, ITestOutputHelper testOutputHelper)
            : base(configurationFixture, testOutputHelper)
        {
            _services = configurationFixture.Services;
        }

        [Theory]
        [InlineData("i don't have url")]
        public void FindTrack_InvalidUrl_ShouldThrowException(string text)
        {
            // arrange
            var streamingManager = new StreamingManager(_services.GetService<IEnumerable<IStreamingService>>());

            // act
            Func<Task> act = () => streamingManager.FindTrackAsync(text);

            // assert
            act.Should().Throw<MatchingStreamingServiceNotFoundException>();
        }

        [Theory]
        [InlineData("https://open.spotify.com/track/4cOdK2wGLETKBW3PvgPWqT", "https://music.yandex.ru/track/610031")]
        public async Task FindTrack_CorrectUrl_ShouldReturnTrack(string source, string target)
        {
            // arrange
            var streamingManager = new StreamingManager(_services.GetService<IEnumerable<IStreamingService>>());

            // act
            var track = await streamingManager.FindTrackAsync(source);

            // assert
            track.Should().NotBeNull();
            track.Url.Should().Be(target);
        }
    }
}
