using System;
using System.Runtime.Serialization;
using BackRoll.Services.Models;

namespace BackRoll.Services.Exceptions
{
    [Serializable]
    public class SameStreamingServiceException : BackRollException
    {
        public StreamingService StreamingService { get; }

        protected SameStreamingServiceException(StreamingService streamingService, ErrorCode errorCode, string message, params object[] args)
            : base(errorCode, message, args)
        {
            StreamingService = streamingService;
        }

        protected SameStreamingServiceException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public static SameStreamingServiceException Create(StreamingService streamingService)
        {
            return new SameStreamingServiceException(streamingService, ErrorCode.SameStreamingService, string.Empty);
        }
    }
}
