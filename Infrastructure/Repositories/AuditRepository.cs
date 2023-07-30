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

    public AuditRepository(IOptions<AuditDbConfiguration> options)
    {
        var settings = MongoClientSettings.FromConnectionString(options.Value.ConnectionString);
        var client = new MongoClient(settings);

        var mongoDatabase = client.GetDatabase(options.Value.DatabaseName);
        _auditCollection = mongoDatabase.GetCollection<AuditPoco>(options.Value.AuditCollectionName);
    }

    public Task SaveAudit(Audit audit)
    {
        return Task.CompletedTask;
    }
}