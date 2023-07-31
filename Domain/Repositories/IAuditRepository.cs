using Domain.Entities;
using Domain.ValueObjects;

namespace Domain.Repositories;

public interface IAuditRepository
{
    Task SaveAudit(Audit audit, CancellationToken cancellationToken);
    
    Task<Audit?> FindById(string id, CancellationToken cancellationToken);
    
    Task<IReadOnlyCollection<Audit>> GetAll(int count, string? lastId, CancellationToken cancellationToken);
    
    Task Remove(string id, CancellationToken cancellationToken);
    
    Task<IReadOnlyCollection<Audit>> DatePeriod(DateTime start, DateTime? end, int count, string? lastId, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<AuditStatPerDay>> GetStatisticsPerDay(CancellationToken cancellationToken);
}