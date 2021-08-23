namespace BackRoll.Services.Abstractions
{
    public interface ISessionService
    {
        string GetAndDeleteLastRequest(long userId);

        void SetLastRequest(long userId, string request);
    }
}
