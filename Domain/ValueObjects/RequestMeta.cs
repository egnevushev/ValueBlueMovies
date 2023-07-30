using System.Net;

namespace Domain.ValueObjects;

public record RequestMeta(
    string SearchToken,
    string ImdbId,
    DateTime Requested,
    DateTime Processed,
    IPAddress IpAddress);