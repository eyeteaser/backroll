using System;
using System.Runtime.Serialization;
using BackRoll.Services.Exceptions;
using Telegram.Bot.Types;

namespace BackRoll.Telegram.Exceptions
{
    [Serializable]
    public class TelegramUserNotFoundException : BackRollException
    {
        public TelegramUserNotFoundException(User user)
            : base(0, $"Configuration for \"{user?.Id}\" user not found")
        {
        }

        protected TelegramUserNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
