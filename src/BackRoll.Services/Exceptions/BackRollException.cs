using System;

namespace BackRoll.Services.Exceptions
{
    public abstract class BackRollException : Exception
    {
        protected BackRollException(string message)
            : base(message)
        {
        }
    }
}
