using System;
using BackRoll.Services.Models;
using BackRoll.Telegram.Configuration;
using BackRoll.Telegram.Data.Entities;
using BackRoll.Telegram.Data.Repositories;
using BackRoll.Telegram.Exceptions;
using FluentAssertions;
using Moq;
using Telegram.Bot.Types;
using Xunit;

namespace Tests.BackRoll.Telegram.Configuration
{
    public class TelegramUserConfigurationTests
    {
        private readonly Mock<ITelegramUserConfigurationRepository> _mockTelegramUserConfigurationRepository;

        public TelegramUserConfigurationTests()
        {
            _mockTelegramUserConfigurationRepository = new Mock<ITelegramUserConfigurationRepository>();
        }

        [Fact]
        public void GetConfiguration_UserIsNull_ShouldThrowArgumentNullException()
        {
            // arrange
            var telegramUserConfiguration = new TelegramUserConfiguration(_mockTelegramUserConfigurationRepository.Object);

            // act
            Action act = () => telegramUserConfiguration.GetConfiguration(null);

            // assert
            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void GetConfiguration_UserDoNotHaveConfiguration_ShouldThrowArgumentNullException()
        {
            // arrange
            _mockTelegramUserConfigurationRepository
                .Setup(x => x.FindByUserId(It.IsAny<long>()))
                .Returns((TelegramUserConfigurationEntity)null);

            var telegramUserConfiguration = new TelegramUserConfiguration(_mockTelegramUserConfigurationRepository.Object);

            // act
            Action act = () => telegramUserConfiguration.GetConfiguration(new User());

            // assert
            act.Should().ThrowExactly<TelegramUserConfigurationNotFoundException>();
        }

        [Fact]
        public void GetConfiguration_UserHaveConfiguration_ShouldReturnCorrectly()
        {
            // arrange
            var entity = new TelegramUserConfigurationEntity() { StreamingService = StreamingService.YandexMusic };
            _mockTelegramUserConfigurationRepository
                .Setup(x => x.FindByUserId(It.IsAny<long>()))
                .Returns(entity);

            var telegramUserConfiguration = new TelegramUserConfiguration(_mockTelegramUserConfigurationRepository.Object);

            // act
            var configuration = telegramUserConfiguration.GetConfiguration(new User());

            // assert
            configuration.Should().NotBeNull();
            configuration.StreamingService.Should().Be(entity.StreamingService);
        }

        [Fact]
        public void SetStreamingService_NoUser_ShouldThrowException()
        {
            // arrange
            var telegramUserConfiguration = new TelegramUserConfiguration(_mockTelegramUserConfigurationRepository.Object);

            // act
            Action act = () => telegramUserConfiguration.SetStreamingService(null, StreamingService.Undefined);

            // assert
            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void SetStreamingService_InvalidStreamingService_ShouldThrowException()
        {
            // arrange
            var telegramUserConfiguration = new TelegramUserConfiguration(_mockTelegramUserConfigurationRepository.Object);

            // act
            Action act = () => telegramUserConfiguration.SetStreamingService(new User(), StreamingService.Undefined);

            // assert
            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void SetStreamingService_ValidData_ShouldSave()
        {
            // arrange
            var telegramUserConfiguration = new TelegramUserConfiguration(_mockTelegramUserConfigurationRepository.Object);

            // act
            telegramUserConfiguration.SetStreamingService(new User(), StreamingService.Spotify);

            // assert
            _mockTelegramUserConfigurationRepository
                .Verify(x => x.Upsert(It.IsAny<TelegramUserConfigurationEntity>()), Times.Once);
        }
    }
}
