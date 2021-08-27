using Telegram.Bot.Types.ReplyMarkups;

namespace BackRoll.Telegram.Scenes
{
    public class SceneResponse
    {
        private SceneResponse(string message, bool isOk, IReplyMarkup replyMarkup, SceneType chainWith)
        {
            Message = message;
            IsOk = isOk;
            ReplyMarkup = replyMarkup;
            ChainWith = chainWith;
        }

        public string Message { get; }

        public IReplyMarkup ReplyMarkup { get; }

        public bool IsOk { get; }

        public SceneType ChainWith { get; }

        public static SceneResponse Ok(string message, IReplyMarkup replyMarkup = null, SceneType chainWith = SceneType.Undefined)
        {
            return new SceneResponse(message, true, replyMarkup, chainWith);
        }

        public static SceneResponse Fail(string message = null, IReplyMarkup replyMarkup = null, SceneType chainWith = SceneType.Undefined)
        {
            return new SceneResponse(message, false, replyMarkup, chainWith);
        }
    }
}
