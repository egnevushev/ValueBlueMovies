using Microsoft.AspNetCore.Mvc;

namespace WebApi.Requests;

public sealed class FindByIdRequest
{
    [FromRoute]
    public string Id { get; set; } = string.Empty;
}