using Domain.Entities;

namespace Domain.Repositories;

public interface IAuditRepository
{
    Task SaveAudit(Audit audit, CancellationToken cancellationToken);
    
    Task<Audit?> FindById(string id, CancellationToken cancellationToken);
    
    Task<IReadOnlyCollection<Audit>> GetAll(CancellationToken cancellationToken);
    
    Task Remove(string id, CancellationToken cancellationToken);
}