namespace Domain.ValueObjects;

public readonly record struct MovieRequest(string Title, Ip IpAddress, DateTime Requested);