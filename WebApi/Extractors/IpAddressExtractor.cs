using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Domain.Exceptions;
using Microsoft.AspNetCore.Http;

namespace WebApi.Extractors;

public static class IpAddressExtractor
{
    private static readonly string[] HeaderNames = new[]
    {
        "X-Forwarded-For",
        "X-Real-IP"
    };
    
    public static IPAddress ExtractIpAddress(this HttpContext context)
    {
        if (context.Connection.RemoteIpAddress is not null)
            return context.Connection.RemoteIpAddress;

        var ip = HeaderNames
            .Select(header => ExtractFromHeader(context, header))
            .FirstOrDefault(x => x is not null);
        
        if (ip is not null)
            return ip;
        
        throw new DomainException("Can't retrieve IpAddress");
    }

    private static IPAddress? ExtractFromHeader(HttpContext context, string headerName)
    {
        if(!context.Request.Headers.TryGetValue(headerName, out var values))
            return null;

        var ips = values.SelectMany(value => value
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim()));
        
        foreach (var ip in ips)
        {
            if (TryParseIpAddress(ip, out var address))
            {
                return address;
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