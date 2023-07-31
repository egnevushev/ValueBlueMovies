using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Repositories.Configuration;
using Infrastructure.Repositories.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Infrastructure.Repositories;

public class AuditRepository : IAuditRepository
{
    private readonly IMongoCollection<AuditPoco> _auditCollection;
    
    public AuditRepository(IMongoClient client, IOptions<AuditDbConfiguration> options)
    {
        var mongoDatabase = client.GetDatabase(options.Value.DatabaseName);
        _auditCollection = mongoDatabase.GetCollection<AuditPoco>(options.Value.AuditCollectionName);
    }
    
    public Task SaveAudit(Audit audit)
    {
        return Task.CompletedTask;
    }
}