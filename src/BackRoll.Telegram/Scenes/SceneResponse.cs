using Telegram.Bot.Types.ReplyMarkups;

namespace BackRoll.Telegram.Scenes
{
    public class SceneResponse
    {
        private SceneResponse(string message, SceneResponseStatus status, IReplyMarkup replyMarkup, SceneType sceneToRedirect = SceneType.Undefined)
        {
            Message = message;
            Status = status;
            ReplyMarkup = replyMarkup;
            SceneToRedirect = sceneToRedirect;
        }

        public string Message { get; }

        public IReplyMarkup ReplyMarkup { get; }

        public SceneResponseStatus Status { get; }

        public SceneType SceneToRedirect { get; }

        public static SceneResponse Ok(string message, IReplyMarkup replyMarkup = null)
        {
            return new SceneResponse(message, SceneResponseStatus.Ok, replyMarkup);
        }

        public static SceneResponse Fail(string message)
        {
            return new SceneResponse(message, SceneResponseStatus.Fail, null);
        }

        public static SceneResponse Redirect(SceneType sceneType)
        {
            return new SceneResponse(null, SceneResponseStatus.Redirect, null, sceneType);
        }
    }
}
