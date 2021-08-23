using Telegram.Bot.Types.ReplyMarkups;

namespace BackRoll.Telegram.Scenes
{
    public class TelegramResponseMessage
    {
        public string Text { get; set; }

        public IReplyMarkup ReplyMarkup { get; set; }
    }
}
