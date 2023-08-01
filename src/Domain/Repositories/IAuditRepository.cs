using Domain.Entities;
using Domain.ValueObjects;

namespace Domain.Repositories;

public interface IAuditRepository
{
    Task SaveAudit(Audit audit, CancellationToken token);
    
    Task<Audit?> FindById(string id, CancellationToken token);
    
    Task<IReadOnlyCollection<Audit>> GetAll(int count, string? lastId, CancellationToken token);
    
    Task Remove(string id, CancellationToken token);
    
    Task<IReadOnlyCollection<Audit>> DatePeriod(DateTime start, DateTime? end, int count, string? lastId, CancellationToken token);

    Task<IReadOnlyCollection<AuditStatPerDay>> GetStatisticsPerDay(DateTime? start, DateTime? end, CancellationToken token);
    
    Task<IpAddressStat> GetRequestsCountByIpAddress(string ipAddress, CancellationToken token);
}