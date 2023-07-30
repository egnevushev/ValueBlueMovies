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
        Task.Run(async () =>
        {
            ILogger? logger = null;
            // Exceptions must be caught
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var repository = scope.ServiceProvider.GetRequiredService<IAuditRepository>();
                logger = scope.ServiceProvider.GetRequiredService<ILogger<AuditService>>();
                
                //create Audit
                //calc
                //var processingTimeMs = (int)(movieRequest.RequestDateTime - DateTime.Now).TotalMilliseconds;

                /*var audit = Domain.Entities.Audit.Create(movieRequest.Title, 
                    movie.ImdbID, 
                    movieRequest.RequestDateTime, 
                    DateTime.Now,
                    movieRequest.IpAddress);*/
        
                //fire and forget or use Quartz
                //repository.SaveAudit(audit);
            }
            catch (Exception e)
            {
                //alert
                logger?.LogError(e, "AAAAA");
            }
        });
    }
}