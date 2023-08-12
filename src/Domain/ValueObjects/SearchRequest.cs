using System.Net;

namespace Domain.ValueObjects;

public readonly record struct SearchRequest(string Title, IPAddress? IpAddress, DateTime Requested);