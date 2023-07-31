using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace WebApi.Authentication;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class ApiKeyAuthFilterAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if(!context.HttpContext.Request.Headers.TryGetValue(AuthConfiguration.ApiKeyHeaderName, out var value))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var options = context.HttpContext.RequestServices.GetRequiredService<IOptions<AuthConfiguration>>();
        var apiKey = options.Value.ApiKey;
        if (!value.Equals(apiKey))
        {
            context.Result = new UnauthorizedResult();
        }
    }
}