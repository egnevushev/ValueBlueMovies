using Domain.Entities;
using Domain.Repositories;

namespace Infrastructure.Repositories;

public class AuditRepository : IAuditRepository
{
    public Task SaveAudit(Audit audit)
    {
        return Task.CompletedTask;
    }
}