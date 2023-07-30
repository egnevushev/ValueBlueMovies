namespace Infrastructure.Cache;

public class CacheConfiguration
{
    public const string SectionName = nameof(CacheConfiguration);

    public long CacheSize { get; set; } = 1024;
    
    public TimeSpan Expiration { get; set; } = TimeSpan.FromMinutes(10);
}