using System;
using System.Linq;
using System.Threading.Tasks;
using BackRoll.Services.Models;
using BackRoll.Telegram.Data.Entities;
using BackRoll.Telegram.Scenes;
using FluentAssertions;
using LiteDB;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Tests.BackRoll.Telegram.Integration.TestsInfrastructure;
using Xunit;

namespace Tests.BackRoll.Telegram.Integration
{
    [Collection(TestsConstants.MainCollectionName)]
    public class ScenesManagerTests
    {
        private readonly IScenesManager _scenesManager;
        private readonly ILiteCollection<TelegramUserConfigurationEntity> _collection;

        public ScenesManagerTests(MainFixture mainFixture)
        {
            _scenesManager = mainFixture.Services.GetService<IScenesManager>();

            var db = mainFixture.Services.GetService<LiteDatabase>();
            _collection = db.GetCollection<TelegramUserConfigurationEntity>("UserConfigurations");
            _collection.DeleteAll();
        }

        [Theory]
        [InlineData("https://music.yandex.ru/track/610031", "https://open.spotify.com/track/4cOdK2wGLETKBW3PvgPWqT")]
        public async Task Process_CorrectSourceUrl_ShouldReturnTargetServiceUrl(string source, string target)
        {
            // arrange
            var configuration = new TelegramUserConfigurationEntity()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = 1,
                StreamingService = StreamingService.Spotify,
            };
            _collection.Upsert(configuration);

            var message = new TelegramMessage()
            {
                From = new User() { Id = configuration.UserId },
                Text = source,
            };

            // act
            var response = await _scenesManager.ProcessAsync(message);

            // assert
            response.Messages.Should().HaveCount(1);
            response.Messages.First().Text.Should().Be(target);
        }

        [Theory]
        [InlineData("https://music.yandex.ru/album/1234214214/track/12341242131")]
        public async Task Process_InvalidUrl_ShouldReturnNotFoundMessage(string source)
        {
            // arrange
            var configuration = new TelegramUserConfigurationEntity()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = 1,
                StreamingService = StreamingService.Spotify,
            };
            _collection.Upsert(configuration);

            var message = new TelegramMessage()
            {
                From = new User() { Id = configuration.UserId },
                Text = source,
            };

            // act
            var response = await _scenesManager.ProcessAsync(message);

            // assert
            response.Messages.Should().HaveCount(1);
            response.Messages.First().Text.Should().NotBeNullOrEmpty();
        }

        [Theory]
        [InlineData("https://music.yandex.ru/album/1234214214/track/12341242131")]
        public async Task Process_NoConfiguration_ShouldReturnSetService(string source)
        {
            // arrange
            var message = new TelegramMessage()
            {
                From = new User() { Id = 1 },
                Text = source,
            };

            // act
            var response = await _scenesManager.ProcessAsync(message);

            // assert
            response.Messages.Should().HaveCount(1);

            var responseMessage = response.Messages.First();
            responseMessage.Text.Should().NotBeNullOrEmpty();
            responseMessage.ReplyMarkup.Should().NotBeNull()
                .And.BeOfType<InlineKeyboardMarkup>();

            var markup = responseMessage.ReplyMarkup as InlineKeyboardMarkup;
            markup.InlineKeyboard.Should().HaveCount(1);

            var keyboardButtons = markup.InlineKeyboard.First();
            keyboardButtons.Should().OnlyContain(x => x.CallbackData.StartsWith("/setservice_"));
            keyboardButtons.Should().Contain(x => x.CallbackData.StartsWith("/setservice_Spotify"));
        }

        [Theory]
        [InlineData("https://music.yandex.ru/track/610031", "https://open.spotify.com/track/4cOdK2wGLETKBW3PvgPWqT")]
        public async Task Process_NoConfigurationAndSetService_ShouldReturnCorrectUrl(string source, string target)
        {
            // arrange
            var user = new User() { Id = 1 };
            var message = new TelegramMessage()
            {
                From = user,
                Text = source,
            };

            // will ask to set service
            var response = await _scenesManager.ProcessAsync(message);
            var spotifyButton = (response.Messages.First().ReplyMarkup as InlineKeyboardMarkup)
                .InlineKeyboard
                .First()
                .First(x => x.CallbackData == "/setservice_Spotify");

            var callback = new TelegramMessage()
            {
                From = user,
                Text = spotifyButton.CallbackData,
            };

            // act
            response = await _scenesManager.ProcessAsync(callback);

            // assert
            response.Messages.Should().HaveCount(2);
            response.Messages[1].Text.Should().Be(target);
        }
    }
}
