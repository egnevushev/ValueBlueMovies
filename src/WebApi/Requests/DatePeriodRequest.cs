using System;

namespace WebApi.Requests;

public sealed class DatePeriodRequest
{
    public int Count { get; set; } = 10;

    public string? LastId { get; set; }
    
    public DateTime Start { get; set; }
    
    public DateTime? End { get; set; }
}