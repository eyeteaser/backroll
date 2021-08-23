using Telegram.Bot.Types;

namespace BackRoll.Telegram.Scenes
{
    public class TelegramMessage
    {
        public User From { get; set; }

        public string Text { get; set; }

        public TelegramMessage()
        {
        }

        public TelegramMessage(User from, string text)
        {
            From = from;
            Text = text;
        }
    }
}
