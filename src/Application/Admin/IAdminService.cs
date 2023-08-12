
using Domain.ValueObjects;

namespace Application.Admin;

public interface IAdminService
{
    Task<Domain.Entities.Audit?> FindById(string id, CancellationToken token);
    
    Task<IReadOnlyCollection<Domain.Entities.Audit>> GetAll(int count, string? lastId, CancellationToken token);
    
    Task Remove(string id, CancellationToken token);
    
    Task<IReadOnlyCollection<Domain.Entities.Audit>> DatePeriod(DateTime start, DateTime? end, int count, string? lastId, CancellationToken token);

    Task<IReadOnlyCollection<Domain.ValueObjects.AuditStatPerDay>> GetStatisticsPerDay(DateTime? start, DateTime? end, CancellationToken token);
    
    Task<IpAddressStat> GetRequestsCountByIpAddress(string ipAddress, CancellationToken token);
}