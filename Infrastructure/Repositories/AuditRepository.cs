using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Repositories.Configuration;
using Infrastructure.Repositories.Models;
using MapsterMapper;
using Microsoft.Extensions.Options;
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
        var cursor = await _auditCollection.FindAsync(x => x.Id == id, options, cancellationToken);
        var poco = await cursor.FirstOrDefaultAsync(cancellationToken);
        
        return poco is null
            ? null
            : _mapper.Map<Audit>(poco);
    }

    public async Task<IReadOnlyCollection<Audit>> GetAll(CancellationToken cancellationToken)
    {
        var options = new FindOptions<AuditPoco> { Sort = Builders<AuditPoco>.Sort.Descending(x => x.Id) };
        var cursor = await _auditCollection.FindAsync(x => true, options, cancellationToken);
        var pocoCollection = await cursor.ToListAsync(cancellationToken);
        
        return _mapper.Map<Audit[]>(pocoCollection);
    }

    public async Task Remove(string id, CancellationToken cancellationToken) =>
        await _auditCollection.DeleteOneAsync(x => x.Id == id, new DeleteOptions(), cancellationToken);
}