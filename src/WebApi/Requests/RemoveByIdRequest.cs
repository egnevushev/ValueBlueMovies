using Microsoft.AspNetCore.Mvc;

namespace WebApi.Requests;

public sealed class RemoveByIdRequest
{
    [FromRoute]
    public string Id { get; set; } = string.Empty;
}