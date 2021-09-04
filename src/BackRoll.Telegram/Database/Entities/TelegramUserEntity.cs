using BackRoll.Services.Models;

namespace BackRoll.Telegram.Database.Entities
{
    public class TelegramUserEntity
    {
        public string Id { get; set; }

        public long UserId { get; set; }

        public StreamingService StreamingService { get; set; }
    }
}
