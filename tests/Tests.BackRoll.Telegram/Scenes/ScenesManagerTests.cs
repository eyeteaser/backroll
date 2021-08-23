using System.Linq;
using System.Threading.Tasks;
using BackRoll.Telegram.Scenes;
using FluentAssertions;
using Moq;
using Xunit;

namespace Tests.BackRoll.Telegram.Scenes
{
    public class ScenesManagerTests
    {
        [Fact]
        public async Task ProcessAsync_SceneReturnedRedirect_ShouldReturnResponseFromSceneToRedirect()
        {
            // arrange
            var message = new TelegramMessage()
            {
                Text = "hey",
            };

            var redirectSceneMock = new Mock<IScene>();
            redirectSceneMock
                .Setup(x => x.ProcessAsync(message))
                .ReturnsAsync(SceneResponse.Fail(chainWith: SceneType.SetService));
            redirectSceneMock
                .Setup(x => x.SceneType)
                .Returns(SceneType.Message);

            string text = "success";
            var setServiceScene = new Mock<IScene>();
            setServiceScene
                .Setup(x => x.ProcessAsync(message))
                .ReturnsAsync(SceneResponse.Ok(text));
            setServiceScene
                .Setup(x => x.SceneType)
                .Returns(SceneType.SetService);

            var scenes = new IScene[] { redirectSceneMock.Object, setServiceScene.Object };

            var sut = new ScenesManager(scenes);

            // act
            var response = await sut.ProcessAsync(message);

            redirectSceneMock
                .Verify(x => x.ProcessAsync(message), Times.Once);

            setServiceScene
                .Verify(x => x.ProcessAsync(message), Times.Once);

            response.Messages.Should().HaveCount(1);
            response.Messages.First().Text.Should().Be(text);
        }
    }
}
