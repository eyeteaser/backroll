using System;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace BackRoll.Services.Exceptions
{
    [Serializable]
    public abstract class BackRollException : Exception
    {
        public ErrorCode ErrorCode { get; }

        public object[] Args { get; }

        public string MessageTemplate { get; }

        protected BackRollException(ErrorCode errorCode, string message, params object[] args)
            : base(Format(message, args))
        {
            ErrorCode = errorCode;
            Args = args;
            MessageTemplate = message;
        }

        protected BackRollException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        private static string Format(string value, params object[] args)
        {
            int pos = 0;
            var fmt = Regex.Replace(
                value,
                @"(?<={)[^}]+(?=})",
                new MatchEvaluator(m => (pos++).ToString()));
            return string.Format(fmt, args);
        }
    }
}
