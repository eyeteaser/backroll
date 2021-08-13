using BackRoll.Services.Exceptions;
using Microsoft.Extensions.Logging;

namespace BackRoll.Telegram.Extensions
{
    public static class LoggerExtensions
    {
        public static void LogInformation(this ILogger logger, BackRollException backRollException)
        {
            logger.LogInformation(backRollException, backRollException.MessageTemplate, backRollException.Args);
        }
    }
}
