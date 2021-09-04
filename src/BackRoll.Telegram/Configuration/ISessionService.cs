using BackRoll.Telegram.Scenes;

namespace BackRoll.Telegram.Configuration
{
    public interface ISessionService
    {
        string GetAndDeleteLastRequest(long userId);

        void SetLastRequest(long userId, string request);

        SceneType GetAndDeleteUnprocessedScene(long userId);

        void SetUnprocessedScene(long userId, SceneType scene);
    }
}
