using System;
using System.Runtime.Serialization;
using BackRoll.Services.Models;

namespace BackRoll.Services.Exceptions
{
    [Serializable]
    public class TrackNotFoundException : BackRollException
    {
        protected TrackNotFoundException(ErrorCode errorCode, string message, params object[] args)
            : base(errorCode, message, args)
        {
        }

        protected TrackNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public static TrackNotFoundException TrackNotFoundByUrl(StreamingService streamingService, string url)
        {
            return new TrackNotFoundException(ErrorCode.TrackNotFoundByUrl, "Url: {Url}, Streaming service: {StreamingService}", url, streamingService);
        }

        public static TrackNotFoundException TrackNotFoundByQuery(StreamingService streamingService, TrackSearchRequest request)
        {
            return new TrackNotFoundException(ErrorCode.TrackNotFoundByQuery, "Query: {Request}, Streaming service: {StreamingService}", request, streamingService);
        }
    }
}
