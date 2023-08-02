namespace Infrastructure.MovieSources.Omdb.Configuration;

public class OmdbConfiguration
{
    public const string SectionName = nameof(OmdbConfiguration);

    public Uri? BaseAddress { get; set; }
    
    public string ApiKey { get; set; } = string.Empty;
    
    public int RetryCount { get; set; } = 3;
    
    public TimeSpan TimeOut { get; set; } = TimeSpan.FromSeconds(30);
    
    public TimeSpan WaitBetweenRetry { get; set; } = TimeSpan.FromSeconds(2);
}