using System.Net;

namespace Domain.ValueObjects;

public sealed class Ip
{
    public string Value { get; private set; }

    private Ip(string value) => Value = value;

    public static Ip Create(IPAddress? ipAddress) => new(ipAddress?.ToString() ?? "unknown");
}