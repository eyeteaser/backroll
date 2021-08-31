using BackRoll.Services.Models;

namespace BackRoll.Telegram.Models
{
    public class TelegramUserModel
    {
        public StreamingService StreamingService { get; set; }

        public bool IsNew { get; set; }
    }
}
