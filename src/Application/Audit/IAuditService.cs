using Domain.ValueObjects;

namespace Application.Audit;

public interface IAuditService
{
    void AuditRequest(RequestMeta requestMeta, CancellationToken token);
}