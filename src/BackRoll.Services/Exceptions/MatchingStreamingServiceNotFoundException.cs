namespace BackRoll.Services.Exceptions
{
    public class MatchingStreamingServiceNotFoundException : BackRollException
    {
        public MatchingStreamingServiceNotFoundException()
            : base("Can't find matching streaming service")
        {
        }
    }
}
