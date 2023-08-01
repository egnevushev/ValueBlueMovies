using System.Net;

namespace Domain.ValueObjects;

public readonly record struct MovieRequest(string Title, IPAddress IpAddress, DateTime Requested);