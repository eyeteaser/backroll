using BackRoll.Services.Models;

namespace BackRoll.Services.Exceptions
{
    public class SameStreamingServiceException : BackRollException
    {
        public StreamingService StreamingService { get; }

        protected SameStreamingServiceException(StreamingService streamingService, ErrorCode errorCode, string message, params object[] args)
            : base(errorCode, message, args)
        {
            StreamingService = streamingService;
        }

        public static SameStreamingServiceException Create(StreamingService streamingService)
        {
            return new SameStreamingServiceException(streamingService, ErrorCode.SameStreamingService, string.Empty);
        }
    }
}
