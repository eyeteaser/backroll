namespace BackRoll.Telegram.Configuration
{
    public class TelegramBotConfig
    {
        public const string CONFIG_SECTION = "TelegramBot";

        public string Secret { get; set; }

        public string DbConnectionString { get; set; } = "./data/telegram.db";
    }
}
