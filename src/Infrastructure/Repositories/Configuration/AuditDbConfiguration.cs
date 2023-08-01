namespace Infrastructure.Repositories.Configuration;

public sealed class AuditDbConfiguration
{
    public const string SectionName = nameof(AuditDbConfiguration);
    
    public string ConnectionString { get; set; } = string.Empty;

    public string DatabaseName { get; set; } = string.Empty;

    public string AuditCollectionName { get; set; } = string.Empty;
}