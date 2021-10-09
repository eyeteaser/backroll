using System;
using System.Threading.Tasks;
using BackRoll.Telegram.Bot;
using FluentAssertions;
using Telegram.Bot.Types;
using Xunit;

namespace Tests.BackRoll.Telegram.Bot
{
    public class TelegramServiceTests
    {
        private readonly TelegramService _sut;

        public TelegramServiceTests()
        {
            _sut = new TelegramService(null, null, null);
        }

        [Fact]
        public async Task ProcessUpdateAsync_UnknownUpdateType_ShouldNotThrowException()
        {
            // arrange
            var update = new Update();

            // act
            Func<Task> act = async () => await _sut.ProcessUpdateAsync(update);

            // assert
            await act.Should().NotThrowAsync();
        }
    }
}
