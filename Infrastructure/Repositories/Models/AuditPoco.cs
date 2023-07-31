using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Infrastructure.Repositories.Models;

public class AuditPoco
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("search_token")] 
    public string SearchToken { get; set; } = string.Empty;
    
    [BsonElement("imdbID")] 
    public string ImdbId { get; set; } = string.Empty;
    
    [BsonElement("processing_time_ms")] 
    public int ProcessingTimeMs { get; set; } 
    
    [BsonElement("timestamp")] 
    public DateTime TimeStamp { get; set; }
    
    [BsonElement("ip_address")] 
    public string IpAddress { get; set; } = string.Empty;
}