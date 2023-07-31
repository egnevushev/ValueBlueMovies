using System.Net;

namespace Domain.Entities;

public sealed record Audit(
    string SearchToken,
    string ImdbId,
    int ProcessingTimeMs,
    DateTime TimeStamp,
    string IpAddress,
    string? Id = null)
{
    public static Audit Create(string searchToken,
        string imdbId,
        DateTime requested,
        DateTime processed,
        IPAddress ipAddress)
    {
        var processingTimeMs = (int)(processed - requested).TotalMilliseconds;
        return new Audit(searchToken, imdbId, processingTimeMs, requested, ipAddress.ToString());
    }
}
