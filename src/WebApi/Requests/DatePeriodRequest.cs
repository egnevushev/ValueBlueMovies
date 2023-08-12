using System;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Requests;

public sealed class DatePeriodRequest
{
    public int Count { get; set; } = 10;

    public string? LastId { get; set; }
    
    [FromRoute]
    public DateTime Start { get; set; }
    
    [FromRoute]
    public DateTime? End { get; set; }
}