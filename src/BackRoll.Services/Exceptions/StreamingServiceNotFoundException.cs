using BackRoll.Services.Models;

namespace BackRoll.Services.Exceptions
{
    public class StreamingServiceNotFoundException : BackRollException
    {
        protected StreamingServiceNotFoundException(ErrorCode errorCode, string message, params object[] args)
            : base(errorCode, message, args)
        {
        }

        public static StreamingServiceNotFoundException MatchingServiceNotFound(string url)
        {
            return new StreamingServiceNotFoundException(ErrorCode.MatchingServiceNotFound, "Can't find matching streaming service for {Url}", url);
        }

        public static StreamingServiceNotFoundException ServiceNotFound(StreamingService streamingService)
        {
            return new StreamingServiceNotFoundException(ErrorCode.ServiceNotFound, "Can't find {StreamingService} streaming service", streamingService);
        }
    }
}
