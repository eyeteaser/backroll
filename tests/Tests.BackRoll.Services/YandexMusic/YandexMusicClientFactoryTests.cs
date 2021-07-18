using BackRoll.Services.YandexMusic;
using FluentAssertions;
using Xunit;

namespace Tests.BackRoll.Services.YandexMusic
{
    public class YandexMusicClientFactoryTests
    {
        [Fact]
        public void CreateClient()
        {
            // arrange
            var config = new YandexMusicConfig();

            // act
            var resolver = YandexMusicClientFactory.CreateYandexMusicClient(config);

            // assert
            resolver.Should().NotBeNull();
        }
    }
}
