using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Tests.BackRoll.Telegram.Integration.TestsInfrastructure;
using Xunit;

namespace Tests.BackRoll.Telegram.Integration
{
    [Collection(TestsConstants.MainCollectionName)]
    public class PingTests
    {
        private readonly HttpClient _client;

        public PingTests(MainFixture mainFixture)
        {
            _client = mainFixture.Client;
        }

        [Fact]
        public async Task Ping()
        {
            // arrange
            // act
            var response = await _client.GetAsync("/ping");

            // assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
