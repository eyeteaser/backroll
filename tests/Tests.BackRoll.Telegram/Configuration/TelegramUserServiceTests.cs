using System;
using BackRoll.Services.Models;
using BackRoll.Telegram.Configuration;
using BackRoll.Telegram.Database.Entities;
using BackRoll.Telegram.Database.Repositories;
using BackRoll.Telegram.Exceptions;
using FluentAssertions;
using Moq;
using Telegram.Bot.Types;
using Xunit;

namespace Tests.BackRoll.Telegram.Configuration
{
    public class TelegramUserServiceTests
    {
        private readonly Mock<ITelegramUserRepository> _mockTelegramUserRepository;
        private readonly TelegramUserService _sut;

        public TelegramUserServiceTests()
        {
            _mockTelegramUserRepository = new Mock<ITelegramUserRepository>();

            _sut = new TelegramUserService(_mockTelegramUserRepository.Object);
        }

        [Fact]
        public void GetConfiguration_UserIsNull_ShouldThrowArgumentNullException()
        {
            // arrange
            // act
            Action act = () => _sut.GetUser(null);

            // assert
            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void GetConfiguration_UserDoNotHaveConfiguration_ShouldThrowArgumentNullException()
        {
            // arrange
            _mockTelegramUserRepository
                .Setup(x => x.FindByUserId(It.IsAny<long>()))
                .Returns((TelegramUserEntity)null);

            // act
            Action act = () => _sut.GetUser(new User());

            // assert
            act.Should().ThrowExactly<TelegramUserNotFoundException>();
        }

        [Fact]
        public void GetConfiguration_UserHaveConfiguration_ShouldReturnCorrectly()
        {
            // arrange
            var entity = new TelegramUserEntity() { StreamingService = StreamingService.YandexMusic };
            _mockTelegramUserRepository
                .Setup(x => x.FindByUserId(It.IsAny<long>()))
                .Returns(entity);

            // act
            var configuration = _sut.GetUser(new User());

            // assert
            configuration.Should().NotBeNull();
            configuration.StreamingService.Should().Be(entity.StreamingService);
        }

        [Fact]
        public void SetStreamingService_NoUser_ShouldThrowException()
        {
            // arrange
            // act
            Action act = () => _sut.SetStreamingService(null, StreamingService.Undefined);

            // assert
            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void SetStreamingService_InvalidStreamingService_ShouldThrowException()
        {
            // arrange
            // act
            Action act = () => _sut.SetStreamingService(new User(), StreamingService.Undefined);

            // assert
            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void SetStreamingService_ValidData_ShouldSave()
        {
            // arrange
            // act
            _sut.SetStreamingService(new User(), StreamingService.Spotify);

            // assert
            _mockTelegramUserRepository
                .Verify(x => x.Upsert(It.IsAny<TelegramUserEntity>()), Times.Once);
        }
    }
}
