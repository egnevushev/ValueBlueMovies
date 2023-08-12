using Microsoft.AspNetCore.Mvc;

namespace WebApi.Requests;

public class GetStatisticsByIpAddressRequest
{
    [FromRoute(Name = "ip")]

    public string IpAddress { get; set; } = string.Empty;
}