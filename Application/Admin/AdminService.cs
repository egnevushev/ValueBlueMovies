using Domain.Exceptions;
using Domain.Repositories;
using Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Application.Admin;

public class AdminService : IAdminService
{
    private readonly IAuditRepository _auditRepository;
    private readonly ILogger<AdminService> _logger;

    public AdminService(IAuditRepository auditRepository, ILogger<AdminService> logger)
    {
        _auditRepository = auditRepository;
        _logger = logger;
    }

    public Task<Domain.Entities.Audit?> FindById(string id, CancellationToken token) 
    {
        try
        {
            return _auditRepository.FindById(id, token);
        }
        catch (DomainException)
        {
            _logger.LogError("Exception has occured when finding by id {id}", id);
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError("Exception has occured when finding by id {id}", id);
            throw new DomainException($"Exception has occured when finding by id {id}", exception);
        }
    }

    public Task<IReadOnlyCollection<Domain.Entities.Audit>> GetAll(int count, string? lastId, CancellationToken token)
    {
        try
        {
            return _auditRepository.GetAll(count, lastId, token);
        }
        catch (DomainException)
        {
            _logger.LogError("Exception has occured when getting all, count: {count}, lastId: {lastId}", count, lastId);
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError("Exception has occured when getting all, count: {count}, lastId: {lastId}", count, lastId);
            throw new DomainException($"Exception has occured when finding by id: {count}, lastId: {lastId}", exception);
        }
    }

    public Task Remove(string id, CancellationToken cancellationToken)
    {
        try
        {
            return _auditRepository.Remove(id, cancellationToken);
        }
        catch (DomainException)
        {
            _logger.LogError("Exception has occured when removing audit: {id}", id);
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError("Exception has occured when removing audit: {id}", id);
            throw new DomainException($"Exception has occured when removing audit: {id}", exception);
        }
    }

    public Task<IReadOnlyCollection<Domain.Entities.Audit>> DatePeriod(DateTime start, DateTime? end, int count, 
        string? lastId, CancellationToken token)
    {
        try
        {
            return _auditRepository.DatePeriod(start, end, count, lastId, token);
        }
        catch (DomainException)
        {
            _logger.LogError("Exception has occured when getting period: {start} - {end}", start, end);
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError("Exception has occured when getting period: {start} - {end}", start, end);
            throw new DomainException($"Exception has occured when getting period: {start} - {end}", exception);
        }
    }
    
    public Task<IReadOnlyCollection<AuditStatPerDay>> GetStatisticsPerDay(CancellationToken token)
    {
        try
        {
            return _auditRepository.GetStatisticsPerDay(token);
        }
        catch (DomainException)
        {
            _logger.LogError("Exception has occured when getting statistics per day");
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError("Exception has occured when getting statistics per day");
            throw new DomainException("Exception has occured when getting statistics per day", exception);
        }
    }
}