## ValueBlueMovie

Service for searching movies and gathering statistics info (Audit).

### Build and run

First, check [`appsettings.json`](src/WebApi/appsettings.json) and replace the following values:



- `AuditDbConfiguration.ConnectionString`: MongoDB connectionString
- `OmdbConfiguration.ApiKey`: Api-Key for [`Omdb API`](http://www.omdbapi.com/apikey.aspx)
- `AuthConfiguration.ApiKey`: Api-Key for admin section

Then run: `./run.sh` or `run.cmd` and go to: [`http://localhost:5050/swagger`](http://localhost:5050/swagger)

### Usage

You can use Swagger for exloring and calling the API enpdoints.  
For access to the admin section you should provide a valid Api-Key with an `X-API-KEY` Header.

### Estimations 
The initial estimation was up to `4-6 hours`:
- 1-2 hours for designing the system
- 3-4 hours for the implementation

The actual time spent was `~10 hours`:  
the majority of the extra time was spent on refreshing my knowledge of MongoDB and authorization, as it had been about 3 years since I last configured it from scratch.

### Architecture overwiev

This is a classic ASP .Net 6 application with Clean Architecture principles:

![films_api drawio (1)](https://github.com/egnevushev/ValueBlueMovies/assets/6730457/79fc139d-e801-44ea-bd71-a07f387faccb)

#### Layers:
- **Domain**: Contains core models, interfaces, custom exceptions, and business logic.
- **Application**: Contains services that represent use cases.
- **Infrastructure**: Contains implementations of all third-party services and MongoDB repository.  
- **Presentation**: a WebApi project, that contains endpoints with Controllers and Validators. It also is a composition root.

This solution meets all assignment requirements:
- Contains two controllers: MovieController for search and AdminController for access to audit and statistics data.
- Stores statistics in MongoDB.
- Flexible: you can easily add more movie sources: just implement a new [`IMovieSource`](src/Domain/Sources/IMovieSource.cs) interface. It would register in DI automatically. We can discuss different approaches to working with a few sources.
- Perform: hot-path (searching movies) is quite performant. The admin part is less performant (we can discuss why and possible steps to improve performance if it would be needed, key points I included below).

I've made some decisions on my own based on a balance between the requirements, performance and time limitations. I would be glad to discuss the reasoning behind each of them. In a real development process, I would discuss this before the implementation.

### Bonus elements

- **UnitTests**. Just for a few modules because of time limitations. In a real application, I would add more tests.
- **Statistics per IpAddress**. An additional Search element in the admin section.
- **Swagger**. You can explore all endpoints and call them (with Api-Key Auth support).
- **Inversion of Control**. I used the default dependency injection implementation in ASP .Net. Moreover, I also used the [`Scrutor`](https://github.com/khellang/Scrutor) library for the register decorator ([`CachedMovieSearchProvider`](src/Domain/MovieSearchProviders/CachedMovieSearchProvider.cs) is a decorator of [`MovieSearchProvider`](src/Domain/MovieSearchProviders/MovieSearchProvider.cs)) and for a scan of all [`IMovieSource`](src/Domain/Sources/IMovieSource.cs) implementations.
- **Validation**. All request models are validated automatically with the [`FluentValidation`](https://github.com/FluentValidation/FluentValidation) library.
- **Caching**. Requests to the external API are cached for 10 minutes (configurable). Cache implementation is behind an interface and can be easily changed from In-Memory to Redis or something different, like the [`BitFaster.Caching`](https://github.com/bitfaster/BitFaster.Caching) library.
- **Perfomance Improvements**. For example, statistics (I called it Audit) fetches under the FireAndForget principle (without waiting for the acknowledgment from a db) to improve the performance. In the future, we can use a queue (or a channel) for statistics gathering. We can use some external libraries like [`Hangfire`](https://github.com/HangfireIO/Hangfire).
- **Pagination**. Methods, that can return a lot of entries, supports pagination to prevent excessive loading.
- **MongoDB Migrations**. I use the [`Mongo.Migration`](https://github.com/SRoddis/Mongo.Migration) library to ensure that all indexes were created in the database before running.
- **Polly**. I use the [`Polly`](https://github.com/App-vNext/Polly) library to prevent exceptions from occasional network failures when fetching a third-party API, like `Omdb`.
- **Mapster** for mapping objects between different layers.
- **Docker support**. You can easily build and run the service with Docker.
- **Problem Details**. There can be two types of exception in Controller: DomainException or ValidationException. The API can issue the following error responses in the [Problem Details specifiction](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.problemdetails): 
    - auth errors will have a 401-status error,
    - ValidationException: 400-status error, 
    - DomainException and all others: 500-status error (w/o stack trace and other details, since they are properly logged and including them in the response is insecure).

### Further steps

#### Statistics optimizations

I have been considering ways to optimize the daily statistics calculations. Now it uses Mongo aggregations on demand. It has suboptimal performance if collections would have a lot of data, but I decided to make it that way because of time limitations and also because the admin, I guess, can wait some time but get the up-to-date data. And I don't expect a lot of requests for statistics. If I'm wrong, there are a few ways to make it more performant:

1. Calculate these statistics every X minutes with BackgroundService and store them in a separate collection. We can use the default ASP .Net BackgroundService for that or some external libraries like [`Hangfire`](https://github.com/HangfireIO/Hangfire) or [`Quartz.NET`](https://github.com/quartznet/quartznet). We don't change existing entries, so we can update the current day only. But there are two issues with this approach:
    - Deletions. We can delete an old entry. To resolve this we can also decrement (-1 request) in statistics per this day in one transaction with deletion.
    - We can lack some data at the end of the day. To prevent this we need to recalculate the previous day at the beginning of the next day to ensure we don't have any lacks in statistics.

    This approach will be more efficient, but statistics data may not be up-to-date.  

    In this case, the design may look as follows:
    ![films_api drawio (2)](https://github.com/egnevushev/ValueBlueMovies/assets/6730457/f6972dee-583f-4629-aad7-4464072215ba)

2. Do not use an aggregation, but increment or decrement requests number every time we save new entries or delete old ones. We also can store two values (number of requests and number of removed entries per day) and calculate the difference on-demand when we need to return statistics.
In this case, we can show up-to-date statistics in the most efficient way, but we lose the flexibility to change the statistics gathering rules in the future.
3. We can use a cache in addition to one of the approaches above.

#### Cache improvements

In case of an increased load, we can face a situation when users can send a lot of search requests with the same title simultaneously. And if we don't have this entry in the cache we will make a lot of the same and unnecessary requests to the API, which costs some money for us. We can eliminate this with locks, for example, or use some libraries for caching like the [`BitFaster.Caching`](https://github.com/bitfaster/BitFaster.Caching).

#### Metrics, alerting + health check

In production-ready systems, it's important to have health checks, metrics, and rules to alert you when something goes wrong. So I would use these to monitor our systmens (for example by means of [OpenTelemetry](https://github.com/open-telemetry/opentelemetry-dotnet), [Prometheus](https://prometheus.io/) and [Grafana](https://grafana.com/)).
