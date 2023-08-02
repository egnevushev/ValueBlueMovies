using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Microsoft.AspNetCore.Http;

namespace WebApi.Extractors;

public static class IpAddressExtractor
{
    private static readonly string[] HeaderNames = new[]
    {
        "X-Forwarded-For",
        "X-Real-IP"
    };
    
    public static IPAddress? ExtractIpAddress(this HttpContext context)
    {
        if (context.Connection.RemoteIpAddress is not null)
            return context.Connection.RemoteIpAddress;

        return HeaderNames
            .Select(header => ExtractFromHeader(context, header))
            .FirstOrDefault(x => x is not null);
    }

    private static IPAddress? ExtractFromHeader(HttpContext context, string headerName)
    {
        if (!context.Request.Headers.TryGetValue(headerName, out var headerValue))
            return null;

        foreach (var ips in headerValue)
        {
            foreach (var ip in ips.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                if (TryParseIpAddress(ip, out var address))
                {
                    return address;
                }
            }
        }

        return null;
    }

    private static bool TryParseIpAddress(string value, [NotNullWhen(true)] out IPAddress? ipAddress)
    {
        ipAddress = null;
        return IPAddress.TryParse(value, out ipAddress) &&
               (ipAddress.AddressFamily is AddressFamily.InterNetwork or AddressFamily.InterNetworkV6);
    }
}