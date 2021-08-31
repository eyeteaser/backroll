﻿using System;
using BackRoll.Services.Models;
using BackRoll.Telegram.Database.Entities;
using BackRoll.Telegram.Database.Repositories;
using BackRoll.Telegram.Exceptions;
using BackRoll.Telegram.Models;
using Telegram.Bot.Types;

namespace BackRoll.Telegram.Configuration
{
    public class TelegramUserService : ITelegramUserService
    {
        private readonly ITelegramUserRepository _telegramUserRepository;

        public TelegramUserService(ITelegramUserRepository telegramUserRepository)
        {
            _telegramUserRepository = telegramUserRepository;
        }

        public TelegramUserModel GetUser(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var telegramUser = _telegramUserRepository.FindByUserId(user.Id);
            if (telegramUser == null)
            {
                throw new TelegramUserNotFoundException(user);
            }

            return new TelegramUserModel()
            {
                StreamingService = telegramUser.StreamingService,
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

            var configuration = _telegramUserRepository.FindByUserId(user.Id);
            if (configuration == null)
            {
                configuration = new TelegramUserEntity()
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = user.Id,
                };
            }

            configuration.StreamingService = streamingService;

            _telegramUserRepository.Upsert(configuration);
        }
    }
}
