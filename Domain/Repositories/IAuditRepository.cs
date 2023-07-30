using Domain.Entities;

namespace Domain.Repositories;

public interface IAuditRepository
{
    Task SaveAudit(Audit audit);
}