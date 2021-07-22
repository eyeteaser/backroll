using Telegram.Bot.Types.ReplyMarkups;

namespace BackRoll.Telegram.Scenes
{
    public class SceneResponse
    {
        private SceneResponse(string message, bool isOk, IReplyMarkup replyMarkup)
        {
            Message = message;
            IsOk = isOk;
            ReplyMarkup = replyMarkup;
        }

        public string Message { get; }

        public IReplyMarkup ReplyMarkup { get; }

        public bool IsOk { get; }

        public static SceneResponse Ok(string message, IReplyMarkup replyMarkup = null)
        {
            return new SceneResponse(message, true, replyMarkup);
        }

        public static SceneResponse Fail(string message)
        {
            return new SceneResponse(message, false, null);
        }
    }
}
