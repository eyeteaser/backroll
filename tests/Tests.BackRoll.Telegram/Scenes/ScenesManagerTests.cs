using System.Threading.Tasks;
using BackRoll.Telegram.Scenes;
using FluentAssertions;
using Moq;
using Telegram.Bot.Types;
using Xunit;

namespace Tests.BackRoll.Telegram.Scenes
{
    public class ScenesManagerTests
    {
        [Fact]
        public async Task ProcessAsync_SceneReturnedRedirect_ShouldReturnResponseFromSceneToRedirect()
        {
            // arrange
            var update = new Update()
            {
                Message = new Message()
                {
                    Text = "hey",
                },
            };

            var redirectSceneMock = new Mock<IScene>();
            redirectSceneMock
                .Setup(x => x.ProcessAsync(update))
                .ReturnsAsync(SceneResponse.Redirect(SceneType.SetService));
            redirectSceneMock
                .Setup(x => x.SceneType)
                .Returns(SceneType.Message);

            var expectedResponse = SceneResponse.Ok("success");
            var setServiceScene = new Mock<IScene>();
            setServiceScene
                .Setup(x => x.ProcessAsync(update))
                .ReturnsAsync(expectedResponse);
            setServiceScene
                .Setup(x => x.SceneType)
                .Returns(SceneType.SetService);

            var scenes = new IScene[] { redirectSceneMock.Object, setServiceScene.Object };

            var sut = new ScenesManager(scenes);

            // act
            var response = await sut.ProcessAsync(update);

            redirectSceneMock
                .Verify(x => x.ProcessAsync(update), Times.Once);

            setServiceScene
                .Verify(x => x.ProcessAsync(update), Times.Once);

            response.Should().Be(expectedResponse);
        }
    }
}
