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

    public async Task<Domain.Entities.Audit?> FindById(string id, CancellationToken token) 
    {
        try
        {
            return await _auditRepository.FindById(id, token);
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

    public async Task<IReadOnlyCollection<Domain.Entities.Audit>> GetAll(int count, string? lastId, CancellationToken token)
    {
        try
        {
            return await _auditRepository.GetAll(count, lastId, token);
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

    public async Task Remove(string id, CancellationToken cancellationToken)
    {
        try
        {
            await _auditRepository.Remove(id, cancellationToken);
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

    public async Task<IReadOnlyCollection<Domain.Entities.Audit>> DatePeriod(DateTime start, DateTime? end, int count, 
        string? lastId, CancellationToken token)
    {
        try
        {
            return await _auditRepository.DatePeriod(start, end, count, lastId, token);
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
    
    public async Task<IReadOnlyCollection<AuditStatPerDay>> GetStatisticsPerDay(DateTime? start, DateTime? end, CancellationToken token)
    {
        try
        {
            return await _auditRepository.GetStatisticsPerDay(start, end, token);
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

    public async Task<IpAddressStat> GetRequestsCountByIpAddress(string ipAddress, CancellationToken token)
    {
        try
        {
            return await _auditRepository.GetRequestsCountByIpAddress(ipAddress, token);
        }
        catch (DomainException)
        {
            _logger.LogError("Exception has occured when getting statistics by ip address {ipAddress}", ipAddress);
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError("Exception has occured when getting statistics per by address {ipAddress}", ipAddress);
            throw new DomainException($"Exception has occured when getting statistics by ip address {ipAddress}", exception);
        }
    }
}