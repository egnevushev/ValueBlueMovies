using Domain.ValueObjects;

namespace Domain.Entities;

public sealed record Audit(
    string? Id,
    string SearchToken,
    string ImdbId,
    int ProcessingTimeMs,
    DateTime TimeStamp,
    string IpAddress)
{
    public static Audit Create(
        string searchToken,
        string imdbId,
        DateTime requested,
        DateTime processed,
        Ip ipAddress)
    {
        var processingTimeMs = (int)(processed - requested).TotalMilliseconds;
        return new Audit(null, searchToken, imdbId, processingTimeMs, requested, ipAddress.Value);
    }
}
