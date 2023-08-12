using System;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Requests;

public class GetStatisticsPerDayRequest
{
    [FromRoute]
    public DateTime? Start { get; set; }
    
    [FromRoute]
    public DateTime? End { get; set; }
}