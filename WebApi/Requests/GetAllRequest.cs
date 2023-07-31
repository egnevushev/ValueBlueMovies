namespace WebApi.Requests;

public sealed class GetAllRequest
{
    public int Count { get; set; } = 10;

    public string? LastId { get; set; }
}