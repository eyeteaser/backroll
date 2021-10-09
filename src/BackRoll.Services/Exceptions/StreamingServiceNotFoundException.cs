using System;
using System.Runtime.Serialization;
using BackRoll.Services.Models;

namespace BackRoll.Services.Exceptions
{
    [Serializable]
    public class StreamingServiceNotFoundException : BackRollException
    {
        protected StreamingServiceNotFoundException(ErrorCode errorCode, string message, params object[] args)
            : base(errorCode, message, args)
        {
        }

        protected StreamingServiceNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
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
