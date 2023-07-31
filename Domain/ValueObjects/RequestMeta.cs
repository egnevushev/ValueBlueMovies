using System.Net;

namespace Domain.ValueObjects;

public readonly record struct RequestMeta(
    string SearchToken,
    string ImdbId,
    DateTime Requested,
    DateTime Processed,
    IPAddress IpAddress);