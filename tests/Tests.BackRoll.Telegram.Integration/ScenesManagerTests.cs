using System;
using System.Threading.Tasks;
using BackRoll.Services.Models;
using BackRoll.Telegram.Data.Entities;
using BackRoll.Telegram.Scenes;
using FluentAssertions;
using LiteDB;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;
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
        public async Task Test(string source, string target)
        {
            // arrange
            var configuration = new TelegramUserConfigurationEntity()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = 1,
                StreamingService = StreamingService.Spotify,
            };
            _collection.Upsert(configuration);

            var update = new Update()
            {
                Message = new Message()
                {
                    From = new User() { Id = configuration.UserId },
                    Text = source,
                },
            };

            // act
            var response = await _scenesManager.ProcessAsync(update);

            // assert
            response.IsOk.Should().BeTrue(response.Message);
            response.Message.Should().Be(target);
        }
    }
}
