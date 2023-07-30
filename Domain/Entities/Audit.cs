using System.Net;

namespace Domain.Entities;

public sealed record Audit(
    string SearchToken,
    string ImdbId,
    int ProcessingTimeMs,
    DateTime TimeStamp,
    string IpAddress,
    string Id = default)
{
    public static Audit Create(
        string searchToken,
        string imdbId,
        DateTime dateRequest,
        IPAddress ipAddress)
    {
        var processingTimeMs = (int)(dateRequest - DateTime.Now).TotalMilliseconds;
        return new Audit(searchToken, imdbId, processingTimeMs, dateRequest, ipAddress.ToString());
    }
    
    public static Audit Create(string searchToken,
        string imdbId,
        DateTime dateRequest,
        int processingTimeMs,
        IPAddress ipAddress)
    {
        return new Audit(searchToken, imdbId, processingTimeMs, dateRequest, ipAddress.ToString());
    }
}
