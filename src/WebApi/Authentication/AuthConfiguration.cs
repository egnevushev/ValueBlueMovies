using System;

namespace WebApi.Authentication;

public class AuthConfiguration
{
    public const string SectionName = nameof(AuthConfiguration);
    
    public const string ApiKeyHeaderName = "X-API-KEY";

    public string ApiKey { get; set; } = String.Empty;
}