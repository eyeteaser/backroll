using System.Threading.Tasks;
using BackRoll.Services.Abstractions;
using BackRoll.Telegram.Configuration;
using BackRoll.Telegram.Exceptions;
using BackRoll.Telegram.Scenes;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Telegram.Bot.Types;
using Xunit;

namespace Tests.BackRoll.Telegram.Scenes
{
    public class MessageSceneTests
    {
        private readonly Mock<ITelegramUserConfiguration> _mockTelegramUserConfiguration;
        private readonly Mock<IStreamingManager> _mockStreamingManager;
        private readonly Mock<ILogger<MessageScene>> _mockLogger;

        public MessageSceneTests()
        {
            _mockTelegramUserConfiguration = new Mock<ITelegramUserConfiguration>();
            _mockStreamingManager = new Mock<IStreamingManager>();
            _mockLogger = new Mock<ILogger<MessageScene>>();
        }

        [Fact]
        public async Task Process_UserDoNotHaveConfiguration_ShouldReturnRedirectToSetServiceScene()
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
                _mockStreamingManager.Object,
                _mockLogger.Object);

            // act
            var response = await messageScene.ProcessAsync(update);

            // assert
            response.Status.Should().Be(SceneResponseStatus.Redirect);
            response.SceneToRedirect.Should().Be(SceneType.SetService);
        }
    }
}
