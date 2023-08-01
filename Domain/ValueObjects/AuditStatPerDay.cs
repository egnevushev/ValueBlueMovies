namespace Domain.ValueObjects;

public record AuditStatPerDay(DateTime Date, long RequestsCount);