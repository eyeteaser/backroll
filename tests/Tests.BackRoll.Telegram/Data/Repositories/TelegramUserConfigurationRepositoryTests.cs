using System;
using BackRoll.Services.Models;
using BackRoll.Telegram.Data.Entities;
using BackRoll.Telegram.Data.Repositories;
using FluentAssertions;
using LiteDB;
using Tests.BackRoll.Telegram.TestsInfrastructure;
using Xunit;

namespace Tests.BackRoll.Telegram.Data.Repositories
{
    public class TelegramUserConfigurationRepositoryTests : IClassFixture<TelegramFixture>
    {
        private readonly LiteDatabase _db;

        public TelegramUserConfigurationRepositoryTests(TelegramFixture telegramFixture)
        {
            _db = telegramFixture.Db;
            _db
                .GetCollection("UserConfigurations")
                .DeleteAll();
        }

        [Fact]
        public void Upsert()
        {
            // arrange
            var configuration = new TelegramUserConfigurationEntity()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = 1,
                StreamingService = StreamingService.Spotify,
            };
            var telegramUserConfigurationRepository = new TelegramUserConfigurationRepository(_db);

            // act
            telegramUserConfigurationRepository.Upsert(configuration);

            // assert
            var collection = _db.GetCollection<TelegramUserConfigurationEntity>("UserConfigurations");
            var dbUser = collection.FindOne(x => x.UserId == configuration.UserId);
            dbUser.Should().NotBeNull();
            dbUser.Id.Should().Be(configuration.Id);
            dbUser.UserId.Should().Be(configuration.UserId);
            dbUser.StreamingService.Should().Be(configuration.StreamingService);
        }

        [Fact]
        public void FindById()
        {
            // arrange
            var configuration = new TelegramUserConfigurationEntity()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = 1,
                StreamingService = StreamingService.Spotify,
            };
            var telegramUserConfigurationRepository = new TelegramUserConfigurationRepository(_db);
            telegramUserConfigurationRepository.Upsert(configuration);

            // act
            var foundConfiguration = telegramUserConfigurationRepository.FindByUserId(configuration.UserId);

            // assert
            foundConfiguration.Should().BeEquivalentTo(configuration);
        }
    }
}
