using Infrastructure.Repositories.Configuration;
using Infrastructure.Repositories.Models;
using Microsoft.Extensions.Options;
using Mongo.Migration.Migrations.Database;
using MongoDB.Driver;

namespace Infrastructure.Migrations;

public class AuditDbIndexesMigration : DatabaseMigration
{
    private readonly string _collectionName;

    public AuditDbIndexesMigration(IOptions<AuditDbConfiguration> options) : base("1.0.0")
    {
        _collectionName = options.Value.ConnectionString;
    }

    public override void Up(IMongoDatabase db)
    {
        var collection = db.GetCollection<AuditPoco>(_collectionName);

        var timeStampIndex = CreateIndexModel(Builders<AuditPoco>.IndexKeys.Ascending(audit => audit.TimeStamp));
        var ipAddressIndex = CreateIndexModel(Builders<AuditPoco>.IndexKeys.Hashed(audit => audit.IpAddress));

        collection.Indexes.CreateMany(new[] { timeStampIndex, ipAddressIndex });
    }

    public override void Down(IMongoDatabase db)
    {
    }

    private static CreateIndexModel<T> CreateIndexModel<T>(IndexKeysDefinition<T> indexKeysDefinition) =>
        new(indexKeysDefinition, new CreateIndexOptions { Background = false });
}