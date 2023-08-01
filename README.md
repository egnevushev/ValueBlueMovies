## ValueBlueMovie

Service for searching movie info and gathering statistics info (Audit).

### Build and run

First, check [`appsettings.json`](src/WebApi/appsettings.json) and provide values in the next sections:  

- `AuditDbConfiguration.ConnectionString`: MongoDB connectionString
- `OmdbConfiguration.ApiKey`: Api-Key for [`Omdb API`](http://www.omdbapi.com/apikey.aspx)
- `AuthConfiguration.ApiKey`: Api-Key for admin section

Then run: `./run.sh` or `run.cmd` and go to: [`http://localhost:5050/swagger`](http://localhost:5050/swagger)

### Usage

You can use swagger for exloring and trying API.  
For access to admin section you should provide a valid Api-Key with a `X-API-KEY` Header.

### Estimations 

After reviewing the assignment, I initially estimated that it would take me around 4-6 hours to complete. I anticipated spending 1-2 hours on the design part and 3-4 hours on coding. However, the actual time it took me to finish the assignment was approximately 10 hours. The majority of the extra time was spent refreshing my knowledge of MongoDB and authorization, as it had been about 3 years since I last configured it from scratch.

### Architecture overwiev

This is a classic ASP .Net 6 application with Clean Architecture principles:

![films_api drawio (1)](https://github.com/egnevushev/ValueBlueMovies/assets/6730457/79fc139d-e801-44ea-bd71-a07f387faccb)

#### Layers:
- **Domain**: Contains core models, interfaces, custom exceptions, and business logic.
- **Application**: Contains services that represent use cases.
- **Infrastructure**: Contains implementations of all third-party services and MongoDB repository.  
- **Presentation**: a WebApi project, that contains endpoints with Controllers and Validators. Also is a composition root.

This solution covers all assignment requirements:
- Contains two controllers: MovieController for search and AdminController for access to audit and statistics data.
- Stores statistics in MongoDB.
- Flexible: you can easily add more movie sources: just implement a new [`IMovieSource`](src/Domain/Sources/IMovieSource.cs) interface. It would register in DI automatically. We can discuss different approaches to working with a few sources.
- Perform: hot-path (searching movies) is quite perform. Admin part is less performed (we can discuss why and steps to make it more perform if it would be needed).

I've made some decisions on my own, and I can explain why I did each of them (some of them because of performance, some of them just the quickest to implement). In a real application, it should be discussed before implementation.

### Bonus elements

- **UnitTests**. Just for a few modules because of time limitations. In a real application, it should be more tests.
- **Statistics per IpAddress**. An additional Search element in the admin section.
- **Swagger**. You can explore all endpoints and try to call them (with ApiKey Auth support).
- **Dependency injection**. I also used the [`Scrutor`](https://github.com/khellang/Scrutor) library for the register decorator ([`CachedMovieSearchProvider`](src/Domain/MovieSearchProviders/CachedMovieSearchProvider.cs) is a decorator of [`MovieSearchProvider`](src/Domain/MovieSearchProviders/MovieSearchProvider.cs)) and for a scan of all [`IMovieSource`](src/Domain/Sources/IMovieSource.cs) implementations.
- **Validation**. All request models are validated automatically with the [`FluentValidation`](https://github.com/FluentValidation/FluentValidation) library.
- **Caching**. Requests to the external API are cached for 10 minutes (configurable). Cache implementation is behind an interface and can be easily changed from In-Memory to Redis or something different, like the [`BitFaster.Caching`](https://github.com/bitfaster/BitFaster.Caching) library.
- **Perfomance Improvings**. For example, statistics (I called it Audit) fetches as a FireAndForget principle (without waiting for ack from db) for improving the performance. In the future, we can use a queue (or a channel) for statistics gathering. We can use some external libraries like [`Hangfire`](https://github.com/HangfireIO/Hangfire).
- **Pagination**. Methods, that can return a lot of entries, supports pagination to prevent too much loading.
- **MongoDB Migrations**. I use the [`Mongo.Migration`](https://github.com/SRoddis/Mongo.Migration) library to ensure that all indexes were created in the database before running.
- **Polly**. I use the [`Polly`](https://github.com/App-vNext/Polly) library to prevent exceptions from occasional network failures with third-party API, like `Omdb`.
- **Mapster** for mapping objects between different layers.
- **Docker support**. You can easily build and run the service with Docker.
- **Problem Details**. In Controller can be whether DomainException or ValidationException. Auth errors occur 401 exceptions, ValidationException occurs 400-status error, DomainException and all others occure 500-status error (without details).

### Further steps

#### Statistics optimizations

I have been considering ways to optimize the daily statistics calculations. Now it uses Mongo aggregations on demand. It has poor performance if collections would has a lot of data, but I decided to make it that way because of time limitations and also because the admin, I guess, can wait some time but get the up-to-date data. And I don't expect a lot of requests for statistics. If I'm wrong, there are a few ways to make it more perform:

1. Calculate these statistics every X minutes with BackgroundService and store them in a separate collection. We can use the default ASP .Net BackgroundService for that or some external libraries like [`Hangfire`](https://github.com/HangfireIO/Hangfire) or [`Quartz.NET`](https://github.com/quartznet/quartznet). We don't change existing entries, so we can update the current day only. But there are two problems with this approach:
    - Deletions. We can delete an old entry. To resolve this we can also decrement (-1 request) in statistics per this day in one transaction with deletion.
    - We can lack some data at the end of the day. To prevent this we need to recalculate the previous day at the beginning of the next day to ensure we don't have any lacks in statistics.

    This approach will be more efficient, but statistics data may not be up-to-date.  

    In this case, the design may look as follows:
    ![films_api drawio (2)](https://github.com/egnevushev/ValueBlueMovies/assets/6730457/f6972dee-583f-4629-aad7-4464072215ba)

1. Do not use an aggregation, but increment or decrement requests amount every time we save new entries or delete old ones. We also can store two values (amount of requests and amount of removed entries per day) and calculate the difference on-demand when we need to return statistics.
    In this case, we can show up-to-date statistics in the most efficient way, but we lose the flexibility to change the statistics gathering rules in the future.
1. We can use a cache in addition to one of the approaches above.

#### Cache improves

In order of increasing load, we can face a situation when users can send a lot of search requests with the same title simultaneously. And if we don't have this entry in the cache we will make a lot of requests to the API, which costs some money for us. We can eliminate this with locks, for example, or use some libraries for caching like the [`BitFaster.Caching`](https://github.com/bitfaster/BitFaster.Caching).

#### Metrics, alerting + health check

In production-ready systems, it's important to have health checks, metrics, and rules to alert you when something goes wrong.
