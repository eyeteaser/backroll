using System.Net;
using System.Threading.Tasks;
using BackRoll.Telegram;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Tests.BackRoll.Telegram.Integration
{
    public class PingTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public PingTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Ping()
        {
            // arrange
            var client = _factory.CreateClient();

            // act
            var response = await client.GetAsync("/ping");

            // assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
