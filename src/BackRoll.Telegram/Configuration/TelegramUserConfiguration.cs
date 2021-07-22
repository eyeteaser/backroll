using System;
using BackRoll.Services.Models;
using BackRoll.Telegram.Data.Entities;
using BackRoll.Telegram.Data.Repositories;
using BackRoll.Telegram.Exceptions;
using BackRoll.Telegram.Models;
using Telegram.Bot.Types;

namespace BackRoll.Telegram.Configuration
{
    public class TelegramUserConfiguration : ITelegramUserConfiguration
    {
        private readonly ITelegramUserConfigurationRepository _telegramUserConfigurationRepository;

        public TelegramUserConfiguration(ITelegramUserConfigurationRepository telegramUserConfigurationRepository)
        {
            _telegramUserConfigurationRepository = telegramUserConfigurationRepository;
        }

        public TelegramUserConfigurationModel GetConfiguration(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var telegramUserConfiguration = _telegramUserConfigurationRepository.FindByUserId(user.Id);
            if (telegramUserConfiguration == null)
            {
                throw new TelegramUserConfigurationNotFoundException(user);
            }

            return new TelegramUserConfigurationModel()
            {
                StreamingService = telegramUserConfiguration.StreamingService,
            };
        }

        public void SetStreamingService(User user, StreamingService streamingService)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (streamingService == StreamingService.Undefined)
            {
                throw new ArgumentException($"{streamingService} is not allowed value", nameof(streamingService));
            }

            var configuration = _telegramUserConfigurationRepository.FindByUserId(user.Id);
            if (configuration == null)
            {
                configuration = new TelegramUserConfigurationEntity()
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = user.Id,
                };
            }

            configuration.StreamingService = streamingService;

            _telegramUserConfigurationRepository.Upsert(configuration);
        }
    }
}
