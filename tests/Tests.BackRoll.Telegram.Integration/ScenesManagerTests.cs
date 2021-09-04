using System;
using System.Linq;
using System.Threading.Tasks;
using BackRoll.Services.Models;
using BackRoll.Telegram.Configuration;
using BackRoll.Telegram.Database.Entities;
using BackRoll.Telegram.Database.Repositories;
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
        private readonly ISessionService _sessionService;
        private readonly ILiteCollection<TelegramUserEntity> _collection;

        public ScenesManagerTests(MainFixture mainFixture)
        {
            _scenesManager = mainFixture.Services.GetService<IScenesManager>();
            _sessionService = mainFixture.Services.GetService<ISessionService>();

            var db = mainFixture.Services.GetService<LiteDatabase>();
            _collection = db.GetCollection<TelegramUserEntity>(TelegramUserRepository.CollectionName);
            _collection.DeleteAll();
        }

        [Theory]
        [InlineData("https://music.yandex.ru/track/610031", "https://open.spotify.com/track/4cOdK2wGLETKBW3PvgPWqT")]
        public async Task Process_CorrectSourceUrl_ShouldReturnTargetServiceUrl(string source, string target)
        {
            // arrange
            var configuration = new TelegramUserEntity()
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
            var configuration = new TelegramUserEntity()
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
            response.Messages.Should().HaveCount(2, TestsHelper.SerializePretty(response.Messages));
            response.Messages[1].Text.Should().Be(target);
        }

        [Theory]
        [InlineData("https://open.spotify.com/track/4cOdK2wGLETKBW3PvgPWqT")]
        public async Task Process_HasSpotifyAsFavoriteServiceAndSendsSpotifyLink_ShouldSuggestOtherPlatformsToGetLink(string source)
        {
            // arrange
            var user = new User() { Id = 1 };
            var configuration = new TelegramUserEntity()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = user.Id,
                StreamingService = StreamingService.Spotify,
            };
            _collection.Upsert(configuration);
            var message = new TelegramMessage()
            {
                From = user,
                Text = source,
            };

            // act
            var response = await _scenesManager.ProcessAsync(message);

            // assert
            response.Messages.Should().HaveCount(1);
            var responseMessage = response.Messages.First();
            responseMessage.ReplyMarkup.Should().NotBeNull(responseMessage.Text)
                .And.BeOfType<InlineKeyboardMarkup>();

            var markup = responseMessage.ReplyMarkup as InlineKeyboardMarkup;
            markup.InlineKeyboard.Should().HaveCount(1);

            var keyboardButtons = markup.InlineKeyboard.First();
            keyboardButtons.Should().OnlyContain(x => x.CallbackData.StartsWith("/message_"));
            keyboardButtons.Should().OnlyContain(x => !x.CallbackData.StartsWith("/message_Spotify"));
            keyboardButtons.Should().Contain(x => x.CallbackData == "/message_YandexMusic");
        }

        [Theory]
        [InlineData("https://open.spotify.com/track/2r1ObHripAsIgOZ0rRwJBy", "https://music.yandex.ru/album/7113863/track/51143747")]
        public async Task Process_HasSpotifyAsFavoriteServiceAndSendsSpotifyLinkAndConvertsItToYandexMusic_ShouldReturnCorrectLink(string source, string target)
        {
            // arrange
            var user = new User() { Id = 1 };
            var configuration = new TelegramUserEntity()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = user.Id,
                StreamingService = StreamingService.Spotify,
            };
            _collection.Upsert(configuration);
            var message = new TelegramMessage()
            {
                From = user,
                Text = source,
            };

            var response = await _scenesManager.ProcessAsync(message);
            string callbackData = (response.Messages.First().ReplyMarkup as InlineKeyboardMarkup)
                .InlineKeyboard.First().First(x => x.CallbackData == "/message_YandexMusic").CallbackData;

            var callback = new TelegramMessage()
            {
                From = user,
                Text = callbackData,
            };

            // act
            response = await _scenesManager.ProcessAsync(callback);

            // assert
            response.Messages.Should().HaveCount(1, TestsHelper.SerializePretty(response.Messages));
            var responseMessage = response.Messages.First();
            responseMessage.Text.Should().Be(target);
        }

        [Fact]
        public async Task Process_StartScene_ShouldReturnWelcomeMessage()
        {
            // arrange
            var user = new User() { Id = 1 };
            var message = new TelegramMessage()
            {
                From = user,
                Text = "/start",
            };

            // act
            var response = await _scenesManager.ProcessAsync(message);

            // assert
            response.Messages.Should().HaveCount(1);
            var responseMessage = response.Messages.First();
            responseMessage.ReplyMarkup.Should().NotBeNull()
                .And.BeOfType<ReplyKeyboardMarkup>();

            _sessionService.GetAndDeleteUnprocessedScene(user.Id);
        }

        [Fact]
        public async Task Process_StartSceneAndSendTrackUrl_ShouldAskToSetFavoriteService()
        {
            // arrange
            var user = new User() { Id = 1 };
            var startMessage = new TelegramMessage()
            {
                From = user,
                Text = "/start",
            };
            var response = await _scenesManager.ProcessAsync(startMessage);
            string trackUrl = (response.Messages.First().ReplyMarkup as ReplyKeyboardMarkup).Keyboard.First().First().Text;
            var message = new TelegramMessage()
            {
                From = user,
                Text = trackUrl,
            };

            // act
            response = await _scenesManager.ProcessAsync(message);

            // assert
            response.Messages.Should().HaveCount(1);
            var responseMessage = response.Messages.First();
            responseMessage.ReplyMarkup.Should().NotBeNull()
                .And.BeOfType<InlineKeyboardMarkup>();
            var markup = responseMessage.ReplyMarkup as InlineKeyboardMarkup;
            markup.InlineKeyboard.First().Should().Contain(x => x.CallbackData == "/setservice_YandexMusic");

            _sessionService.GetAndDeleteUnprocessedScene(user.Id);
        }

        [Fact]
        public async Task Process_StartSceneAndSendTrackUrlAndConfigureFavoriteService_ShouldReturnTrackUrlAndFinalConfigurationMessage()
        {
            // arrange
            var user = new User() { Id = 1 };
            var startMessage = new TelegramMessage()
            {
                From = user,
                Text = "/start",
            };
            var response = await _scenesManager.ProcessAsync(startMessage);

            string trackUrl = (response.Messages.First().ReplyMarkup as ReplyKeyboardMarkup).Keyboard.First().First().Text;
            var trackMessage = new TelegramMessage()
            {
                From = user,
                Text = trackUrl,
            };
            response = await _scenesManager.ProcessAsync(trackMessage);

            string callbackData = (response.Messages.First().ReplyMarkup as InlineKeyboardMarkup)
                .InlineKeyboard.First().First(x => x.CallbackData == "/setservice_YandexMusic").CallbackData;
            var message = new TelegramMessage()
            {
                From = user,
                Text = callbackData,
            };

            // act
            response = await _scenesManager.ProcessAsync(message);

            // assert
            response.Messages.Should().HaveCount(3);

            var favoriteServiceMessage = response.Messages.First();
            favoriteServiceMessage.Text.Should().NotBeNullOrEmpty();

            var trackResponseMessage = response.Messages[1];
            trackResponseMessage.Text.Should().NotBeNullOrEmpty();

            var finishedConfigurationMessage = response.Messages[2];
            finishedConfigurationMessage.Text.Should().NotBeNullOrEmpty();

            _sessionService.GetAndDeleteUnprocessedScene(user.Id);
        }

        [Fact]
        public async Task Process_StartSceneAndSendTrackUrlAndConfigureFavoriteServiceAndSendOneMoreMessage_ShouldReturnOnlyOneMessage()
        {
            // arrange
            var user = new User() { Id = 1 };
            var startMessage = new TelegramMessage()
            {
                From = user,
                Text = "/start",
            };
            var response = await _scenesManager.ProcessAsync(startMessage);

            string trackUrl = (response.Messages.First().ReplyMarkup as ReplyKeyboardMarkup).Keyboard.First().First().Text;
            var trackMessage = new TelegramMessage()
            {
                From = user,
                Text = trackUrl,
            };
            response = await _scenesManager.ProcessAsync(trackMessage);

            string callbackData = (response.Messages.First().ReplyMarkup as InlineKeyboardMarkup)
                .InlineKeyboard.First().First(x => x.CallbackData == "/setservice_YandexMusic").CallbackData;
            var message = new TelegramMessage()
            {
                From = user,
                Text = callbackData,
            };

            await _scenesManager.ProcessAsync(message);

            // act
            response = await _scenesManager.ProcessAsync(trackMessage);

            // assert
            response.Messages.Should().HaveCount(1);

            _sessionService.GetAndDeleteUnprocessedScene(user.Id);
        }
    }
}
