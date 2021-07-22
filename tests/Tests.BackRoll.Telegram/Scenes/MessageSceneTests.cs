using System.Threading.Tasks;
using BackRoll.Services.Abstractions;
using BackRoll.Telegram.Configuration;
using BackRoll.Telegram.Exceptions;
using BackRoll.Telegram.Scenes;
using FluentAssertions;
using Moq;
using Telegram.Bot.Types;
using Xunit;

namespace Tests.BackRoll.Telegram.Scenes
{
    public class MessageSceneTests
    {
        private readonly Mock<ITelegramUserConfiguration> _mockTelegramUserConfiguration;
        private readonly Mock<IStreamingManager> _mockStreamingManager;

        public MessageSceneTests()
        {
            _mockTelegramUserConfiguration = new Mock<ITelegramUserConfiguration>();
            _mockStreamingManager = new Mock<IStreamingManager>();
        }

        [Fact]
        public async Task Process_UserDoNotHaveConfiguration_ShouldReturnFailedResponse()
        {
            // arrange
            var update = new Update()
            {
                Message = new Message(),
            };

            _mockTelegramUserConfiguration
                .Setup(x => x.GetConfiguration(It.IsAny<User>()))
                .Throws(new TelegramUserConfigurationNotFoundException(null));

            var messageScene = new MessageScene(
                _mockTelegramUserConfiguration.Object,
                _mockStreamingManager.Object);

            // act
            var response = await messageScene.ProcessAsync(update);

            // assert
            response.IsOk.Should().BeFalse();
        }
    }
}
