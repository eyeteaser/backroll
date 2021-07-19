namespace BackRoll.Services.YandexMusic
{
    public class YandexMusicConfig
    {
        public const string CONFIG_SECTION = "YandexMusic";

        public bool UseProxy { get; set; }

        public string ProxyHost { get; set; }

        public string ProxyPort { get; set; }

        public string ProxyUserName { get; set; }

        public string ProxyPassword { get; set; }
    }
}
