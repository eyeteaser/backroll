using BackRoll.Telegram.Configuration;
using FluentAssertions;
using Xunit;

namespace Tests.BackRoll.Telegram.Configuration
{
    public class InMemorySessionServiceTests
    {
        private readonly InMemorySessionService _sut;

        public InMemorySessionServiceTests()
        {
            _sut = new InMemorySessionService();
        }

        [Fact]
        public void SetLastRequest_NoRequestsBefore_ShouldAddRequestInSession()
        {
            // arrange
            var userId = 1;
            var request = "https://open.spotify.com/track/4cOdK2wGLETKBW3PvgPWqT";

            // act
            _sut.SetLastRequest(userId, request);

            // assert
            _sut.GetAndDeleteLastRequest(userId).Should().NotBeNullOrEmpty()
                .And.Be(request);

            _sut.GetAndDeleteLastRequest(userId).Should().BeNull();
        }

        [Fact]
        public void SetLastRequest_HaveRequestBefore_ShouldOverriderRequest()
        {
            // arrange
            var userId = 1;
            _sut.SetLastRequest(userId, "https://open.spotify.com/track/0nrRP2bk19rLc0orkWPQk2");

            var request = "https://open.spotify.com/track/4cOdK2wGLETKBW3PvgPWqT";

            // act
            _sut.SetLastRequest(userId, request);

            // assert
            _sut.GetAndDeleteLastRequest(userId).Should().NotBeNullOrEmpty()
                .And.Be(request);

            _sut.GetAndDeleteLastRequest(userId).Should().BeNull();
        }
    }
}
