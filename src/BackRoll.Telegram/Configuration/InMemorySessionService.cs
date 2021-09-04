using System.Collections.Concurrent;
using BackRoll.Telegram.Scenes;

namespace BackRoll.Telegram.Configuration
{
    public class InMemorySessionService : ISessionService
    {
        private readonly ConcurrentDictionary<long, UserSessionData> _session;

        public InMemorySessionService()
        {
            _session = new ConcurrentDictionary<long, UserSessionData>();
        }

        public string GetAndDeleteLastRequest(long userId)
        {
            var sessionData = GetSessionData(userId);
            var lastRequest = sessionData.LastRequest;
            sessionData.LastRequest = null;
            return lastRequest;
        }

        public void SetLastRequest(long userId, string request)
        {
            var sessionData = GetSessionData(userId);
            sessionData.LastRequest = request;
        }

        public SceneType GetAndDeleteUnprocessedScene(long userId)
        {
            var sessionData = GetSessionData(userId);
            var unprocessedScene = sessionData.UnprocessedScene;
            sessionData.UnprocessedScene = SceneType.Undefined;
            return unprocessedScene;
        }

        public void SetUnprocessedScene(long userId, SceneType scene)
        {
            var sessionData = GetSessionData(userId);
            sessionData.UnprocessedScene = scene;
        }

        private UserSessionData GetSessionData(long userId)
        {
            return _session.GetOrAdd(userId, new UserSessionData());
        }
    }
}
