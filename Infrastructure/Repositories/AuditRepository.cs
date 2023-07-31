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
    
    public async Task SaveAudit(Audit audit, CancellationToken cancellationToken)
    {
        var poco = _mapper.Map<AuditPoco>(audit);
        await _auditCollection.InsertOneAsync(poco, new InsertOneOptions(), cancellationToken);
    }

    public async Task<Audit?> FindById(string id, CancellationToken cancellationToken)
    {
        var options = new FindOptions<AuditPoco> { Limit = 1 };
        var cursor = await _auditCollection.FindAsync(x => x.Id == ObjectId.Parse(id), options, cancellationToken);
        var poco = await cursor.FirstOrDefaultAsync(cancellationToken);
        
        return poco is null
            ? null
            : _mapper.Map<Audit>(poco);
    }

    public async Task<IReadOnlyCollection<Audit>> GetAll(int count, string? lastId, CancellationToken cancellationToken)
    {
        var options = new FindOptions<AuditPoco>
            { Sort = Builders<AuditPoco>.Sort.Descending(x => x.Id), Limit = count };
        
        Expression<Func<AuditPoco, bool>> where = lastId is null
            ? _ => true
            : audit => audit.Id < ObjectId.Parse(lastId);
        
        var cursor = await _auditCollection.FindAsync(where, options, cancellationToken);
        var pocoCollection = await cursor.ToListAsync(cancellationToken);
        
        return _mapper.Map<Audit[]>(pocoCollection);
    }

    public async Task Remove(string id, CancellationToken cancellationToken) =>
        await _auditCollection.DeleteOneAsync(x => x.Id == ObjectId.Parse(id), new DeleteOptions(), cancellationToken);

    public async Task<IReadOnlyCollection<Audit>> DatePeriod(DateTime start, DateTime? end, int count, string? lastId, 
        CancellationToken cancellationToken)
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
        
        var cursor = await _auditCollection.FindAsync(filter.And(filterDefinitions), options, cancellationToken);
        var pocoCollection = await cursor.ToListAsync(cancellationToken);

        return _mapper.Map<Audit[]>(pocoCollection);
    }

    public async Task<IReadOnlyCollection<AuditStatPerDay>> GetStatisticsPerDay(CancellationToken cancellationToken)
    {
        return await _auditCollection
            .Aggregate()
            .Group(a => new DateTime(a.TimeStamp.Year, a.TimeStamp.Month, a.TimeStamp.Day, 0, 0, 0),
                group => new AuditStatPerDay(group.Key, group.Count())
            )
            .ToListAsync(cancellationToken);
    }
}