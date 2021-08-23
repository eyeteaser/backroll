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
        private readonly Mock<ISessionService> _mockSessionService;
        private readonly Mock<ILogger<MessageScene>> _mockLogger;

        public MessageSceneTests()
        {
            _mockTelegramUserConfiguration = new Mock<ITelegramUserConfiguration>();
            _mockStreamingManager = new Mock<IStreamingManager>();
            _mockSessionService = new Mock<ISessionService>();
            _mockLogger = new Mock<ILogger<MessageScene>>();
        }

        [Fact]
        public async Task Process_UserDoNotHaveConfiguration_ShouldReturnRedirectToSetServiceScene()
        {
            // arrange
            var message = new TelegramMessage()
            {
                From = new User()
                {
                    Id = 1,
                },
            };

            _mockTelegramUserConfiguration
                .Setup(x => x.GetConfiguration(It.IsAny<User>()))
                .Throws(new TelegramUserConfigurationNotFoundException(null));

            var messageScene = new MessageScene(
                _mockTelegramUserConfiguration.Object,
                _mockStreamingManager.Object,
                _mockSessionService.Object,
                _mockLogger.Object);

            // act
            var response = await messageScene.ProcessAsync(message);

            // assert
            response.IsOk.Should().BeFalse();
            response.ChainWith.Should().Be(SceneType.SetService);
        }
    }
}
