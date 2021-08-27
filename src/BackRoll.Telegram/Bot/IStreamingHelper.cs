using BackRoll.Services.Models;
using Telegram.Bot.Types.ReplyMarkups;

namespace BackRoll.Telegram.Bot
{
    public interface IStreamingHelper
    {
        string GetStreamingPrettyName(StreamingService streamingService);

        IReplyMarkup CreateStreamingButtonsMarkup(string commandPrefix, StreamingService exclude = StreamingService.Undefined);

        bool TryParseStreamingData(string commandPrefix, string data, out StreamingService streamingService);
    }
}
