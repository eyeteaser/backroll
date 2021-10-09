using System;
using System.Runtime.Serialization;
using BackRoll.Services.Models;

namespace BackRoll.Services.Exceptions
{
    [Serializable]
    public class WrongTrackFoundException : BackRollException
    {
        public Track Found { get; }

        protected WrongTrackFoundException(Track found, ErrorCode errorCode, string message, params object[] args)
            : base(errorCode, message, args)
        {
            Found = found;
        }

        protected WrongTrackFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public static WrongTrackFoundException Create(Track original, Track found)
        {
            return new WrongTrackFoundException(found, ErrorCode.WrongTrackFound, "Wrong track found. Original: {Original}, Found: {Found}", original, found);
        }
    }
}
