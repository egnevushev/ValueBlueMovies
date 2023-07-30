using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Infrastructure.Repositories.Models;


public class AuditPoco
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    private string Id { get; set; } = string.Empty;

    [BsonElement("search_token")] 
    private string SearchToken { get; set; } = string.Empty;
    
    [BsonElement("imdbID")] 
    string ImdbId{ get; set; } = string.Empty;
    
    [BsonElement("processing_time_ms")] 
    int ProcessingTimeMs{ get; set; } 
    
    [BsonElement("timestamp")] 
    DateTime TimeStamp{ get; set; }
    
    [BsonElement("ip_address")] 
    string IpAddress{ get; set; } = string.Empty;
}