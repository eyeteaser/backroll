using BackRoll.Services.Models;

namespace BackRoll.Services.Exceptions
{
    public class StreamingServiceNotFoundException : BackRollException
    {
        public const int MatchingServiceNotFound = 1;
        public const int ServiceNotFound = 2;

        protected StreamingServiceNotFoundException(int errorCode, string message)
            : base(errorCode, message)
        {
        }

        public static StreamingServiceNotFoundException MatchingServiceNotFoundException()
        {
            return new StreamingServiceNotFoundException(MatchingServiceNotFound, "Can't find matching streaming service");
        }

        public static StreamingServiceNotFoundException ServiceNotFoundException(StreamingService streamingService)
        {
            return new StreamingServiceNotFoundException(ServiceNotFound, $"Can't find {streamingService} streaming service with");
        }
    }
}
