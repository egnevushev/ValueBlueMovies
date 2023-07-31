
namespace Application.Admin;

public interface IAdminService
{
    Task<Domain.Entities.Audit> FindById(string id, CancellationToken cancellationToken);
    
    Task<IReadOnlyCollection<Domain.Entities.Audit>> GetAll(CancellationToken cancellationToken);
    
    Task Remove(string id, CancellationToken cancellationToken);
}