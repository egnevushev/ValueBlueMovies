using System.Linq.Expressions;
using Domain.Entities;
using Domain.Repositories;
using Domain.ValueObjects;
using Infrastructure.Repositories.Configuration;
using Infrastructure.Repositories.Models;
using MapsterMapper;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Infrastructure.Repositories;

public class AuditRepository : IAuditRepository
{
    private readonly IMongoCollection<AuditPoco> _auditCollection;
    private readonly IMapper _mapper;
    
    public AuditRepository(IMongoClient client, IMapper mapper, IOptions<AuditDbConfiguration> options)
    {
        var mongoDatabase = client.GetDatabase(options.Value.DatabaseName);
        _auditCollection = mongoDatabase.GetCollection<AuditPoco>(options.Value.AuditCollectionName);
        _mapper = mapper;
    }
    
    public async Task SaveAudit(Audit audit, CancellationToken token)
    {
        var poco = _mapper.Map<AuditPoco>(audit);
        await _auditCollection.InsertOneAsync(poco, new InsertOneOptions(), token);
    }

    public async Task<Audit?> FindById(string id, CancellationToken token)
    {
        var options = new FindOptions<AuditPoco> { Limit = 1 };
        var cursor = await _auditCollection.FindAsync(x => x.Id == ObjectId.Parse(id), options, token);
        var poco = await cursor.FirstOrDefaultAsync(token);
        
        return poco is null
            ? null
            : _mapper.Map<Audit>(poco);
    }

    public async Task<IReadOnlyCollection<Audit>> GetAll(int count, string? lastId, CancellationToken token)
    {
        var options = new FindOptions<AuditPoco>
            { Sort = Builders<AuditPoco>.Sort.Descending(x => x.Id), Limit = count };
        
        Expression<Func<AuditPoco, bool>> where = lastId is null
            ? _ => true
            : audit => audit.Id < ObjectId.Parse(lastId);
        
        var cursor = await _auditCollection.FindAsync(where, options, token);
        var pocoCollection = await cursor.ToListAsync(token);
        
        return _mapper.Map<Audit[]>(pocoCollection);
    }

    public async Task Remove(string id, CancellationToken token) =>
        await _auditCollection.DeleteOneAsync(x => x.Id == ObjectId.Parse(id), new DeleteOptions(), token);

    public async Task<IReadOnlyCollection<Audit>> DatePeriod(DateTime start, DateTime? end, int count, string? lastId, CancellationToken token)
    {
        var sorting = Builders<AuditPoco>.Sort
            .Descending(x => x.TimeStamp)
            .Descending(x => x.Id);
        var options = new FindOptions<AuditPoco> { Sort = sorting, Limit = count };

        var filter = Builders<AuditPoco>.Filter;

        List<FilterDefinition<AuditPoco>> filterDefinitions = new ()
        {
            filter.Gte(x => x.TimeStamp, start)
        };

        if (end is not null)
        {
            filterDefinitions.Add(filter.Lt(x => x.TimeStamp, end.Value));
        }
        
        if (lastId is not null)
        {
            filterDefinitions.Add(filter.Lt(x => x.Id, new ObjectId(lastId)));
        }
        
        var cursor = await _auditCollection.FindAsync(filter.And(filterDefinitions), options, token);
        var pocoCollection = await cursor.ToListAsync(token);

        return _mapper.Map<Audit[]>(pocoCollection);
    }

    public async Task<IReadOnlyCollection<AuditStatPerDay>> GetStatisticsPerDay(DateTime? start, DateTime? end, CancellationToken token)
    {
        Expression<Func<AuditPoco, bool>> match = (start, end) switch
        {
            (null, null) => _ => true,
            (not null, null) => (audit) => audit.TimeStamp >= start,
            (null, not null) => (audit) => audit.TimeStamp <= end.Value,
            (not null, not null) => (audit) => audit.TimeStamp >= start && audit.TimeStamp <= end.Value
        };
            
        return await _auditCollection
            .Aggregate()
            .Match(match)
            .Group(a => new DateTime(a.TimeStamp.Year, a.TimeStamp.Month, a.TimeStamp.Day, 0, 0, 0),
                group => new AuditStatPerDay(group.Key, group.Count())
            )
            .Sort(Builders<AuditStatPerDay>.Sort.Descending(x => x.Date))
            .ToListAsync(token);
    }

    public async Task<IpAddressStat> GetRequestsCountByIpAddress(string ipAddress, CancellationToken token)
    {
        var options = new CountOptions();
        var count =  await _auditCollection.CountDocumentsAsync(x => x.IpAddress == ipAddress, options, token);
        return new IpAddressStat(ipAddress, count);
    }
}