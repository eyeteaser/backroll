using System.Collections.Concurrent;
using BackRoll.Services.Abstractions;

namespace BackRoll.Services.Services
{
    public class InMemorySessionService : ISessionService
    {
        private readonly ConcurrentDictionary<long, string> _session;

        public InMemorySessionService()
        {
            _session = new ConcurrentDictionary<long, string>();
        }

        public string GetAndDeleteLastRequest(long userId)
        {
            _session.TryRemove(userId, out string lastRequest);
            return lastRequest;
        }

        public void SetLastRequest(long userId, string request)
        {
            _session.AddOrUpdate(userId, request, (id, existingRequest) => request);
        }
    }
}
