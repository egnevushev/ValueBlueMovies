using Domain.ValueObjects;

namespace Domain.Audit;

public interface IAuditService
{
    void AuditRequest(RequestMeta requestMeta, CancellationToken cancellationToken);
}