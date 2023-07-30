using Domain.Entities;

namespace Domain.Repositories;

public interface IAuditAdminRepository
{
    Task<Audit?> FindById(Guid id);
    
    Task DeleteById(Guid id);
    
    // .. more
}