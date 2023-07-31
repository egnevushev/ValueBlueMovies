using Domain.Repositories;

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

    public Task<IReadOnlyCollection<Domain.Entities.Audit>> GetAll(CancellationToken cancellationToken) =>
        _auditRepository.GetAll(cancellationToken);

    public Task Remove(string id, CancellationToken cancellationToken) =>
        _auditRepository.Remove(id, cancellationToken);

}