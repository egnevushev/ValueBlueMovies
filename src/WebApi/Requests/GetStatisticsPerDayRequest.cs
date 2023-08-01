using System;

namespace WebApi.Requests;

public class GetStatisticsPerDayRequest
{
    public DateTime? Start { get; set; }
    
    public DateTime? End { get; set; }
}