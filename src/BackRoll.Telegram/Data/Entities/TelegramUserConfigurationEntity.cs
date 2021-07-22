using BackRoll.Services.Models;

namespace BackRoll.Telegram.Data.Entities
{
    public class TelegramUserConfigurationEntity
    {
        public string Id { get; set; }

        public long UserId { get; set; }

        public StreamingService StreamingService { get; set; }
    }
}
