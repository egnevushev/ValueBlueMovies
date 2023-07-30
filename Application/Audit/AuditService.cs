using Domain.Repositories;
using Domain.ValueObjects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Application.Audit;

public class AuditService : IAuditService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public AuditService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public void AuditRequest(RequestMeta requestMeta)
    {
        //todo: should split responsibility of FireAndForget and Auditing
        
        // FIRE AND FORGET 
        Task.Run(async () =>
        {
            ILogger? logger = null;
            
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var repository = scope.ServiceProvider.GetRequiredService<IAuditRepository>();
                logger = scope.ServiceProvider.GetRequiredService<ILogger<AuditService>>();
                
                var processingTimeMs = (int)(requestMeta.Processed - requestMeta.Requested).TotalMilliseconds;

                var audit = Domain.Entities.Audit.Create(requestMeta.SearchToken, 
                    requestMeta.ImdbId, 
                    requestMeta.Requested, 
                    processingTimeMs,
                    requestMeta.IpAddress);
        
                await repository.SaveAudit(audit);
            }
            catch (Exception e)
            {
                //alert
                logger?.LogError(e, "Error has occured while auditing request: {@Request}", requestMeta);
            }
        });
    }
}