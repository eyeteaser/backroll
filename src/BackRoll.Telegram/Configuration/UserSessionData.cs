using BackRoll.Telegram.Scenes;

namespace BackRoll.Telegram.Configuration
{
    public class UserSessionData
    {
        public string LastRequest { get; set; }

        public SceneType UnprocessedScene { get; set; }
    }
}
