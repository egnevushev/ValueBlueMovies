using System.Net;

namespace Domain.ValueObjects;

public sealed class Ip
{
    private const string Unknown = "unknown";
    private const string Localhost = "127.0.0.1";
    
    public string Value { get; private set; }

    private Ip(string value) => Value = value;

    public static Ip Create(IPAddress? ipAddress)
    {
        var ip = ipAddress?.ToString();
        return ip switch
        {
            null => new Ip(Unknown),
            "::1" => new Ip(Localhost),
            _ => new Ip(ip)
        };
    }
}