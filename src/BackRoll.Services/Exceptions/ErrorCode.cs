namespace BackRoll.Services.Exceptions
{
    public enum ErrorCode
    {
        Undefined = 0,
        MatchingServiceNotFound = 1,
        ServiceNotFound,
        TrackNotFoundByUrl,
        TrackNotFoundByQuery,
    }
}
