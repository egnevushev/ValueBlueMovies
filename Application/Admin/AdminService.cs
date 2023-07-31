using Domain.Repositories;
using Domain.ValueObjects;

namespace Application.Admin;

public class AdminService : IAdminService
{
    private readonly IAuditRepository _auditRepository;

    public AdminService(IAuditRepository auditRepository)
    {
        _auditRepository = auditRepository;
    }

    public Task<Domain.Entities.Audit?> FindById(string id, CancellationToken cancellationToken) =>
        _auditRepository.FindById(id, cancellationToken);

    public Task<IReadOnlyCollection<Domain.Entities.Audit>> GetAll(int count, string? lastId,
        CancellationToken cancellationToken) =>
        _auditRepository.GetAll(count, lastId, cancellationToken);

    public Task Remove(string id, CancellationToken cancellationToken) =>
        _auditRepository.Remove(id, cancellationToken);

    public Task<IReadOnlyCollection<Domain.Entities.Audit>> DatePeriod(DateTime start, DateTime? end, int count,
        string? lastId, CancellationToken cancellationToken) =>
        _auditRepository.DatePeriod(start, end, count, lastId, cancellationToken);

    public Task<IReadOnlyCollection<AuditStatPerDay>> GetStatisticsPerDay(CancellationToken cancellationToken) =>
        _auditRepository.GetStatisticsPerDay(cancellationToken);
}