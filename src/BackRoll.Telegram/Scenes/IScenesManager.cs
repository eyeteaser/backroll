using System.Threading.Tasks;

namespace BackRoll.Telegram.Scenes
{
    public interface IScenesManager
    {
        Task<TelegramResponse> ProcessAsync(TelegramMessage message);
    }
}
