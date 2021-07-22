using System;

namespace BackRoll.Services.Exceptions
{
    public abstract class BackRollException : Exception
    {
        public int ErrorCode { get; }

        protected BackRollException(int errorCode, string message)
            : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}
