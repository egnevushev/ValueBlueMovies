## ValueBlueMovie

Service for searching movie info and gathering statistics info (Audit).

### Build and run

First, check `appsettings.json` and provide values in the next sections:  

- `AuditDbConfiguration.ConnectionString`: MongoDB connectionString
- `OmdbConfiguration.ApiKey`: Api-Key for Omdb API
- `AuthConfiguration.ApiKey`: Api-Key for admin section

Then build docker image: `./build.sh` and run: `./run.sh`.  
Go to: `https://localhost:5050`

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
- Flexible: you can easily add more movie sources: just implement a new `IMovieSource` interface. It would register in DI automatically. We can discuss different approaches to working with a few sources.
- Perform: hot-path (searching movies) is quite perform. Admin part is less performed (we can discuss why and steps to make it more perform if it would be needed).

I've made some decisions on my own, and I can explain why I did each of them (some of them because of performance, some of them just the quickest to implement). In a real application, it should be discussed before implementation.

### Bonus elements

- **UnitTests**. Just for a few modules because of time limitations. In a real application, it should be more tests.
- **Statistics per IpAddress**. An additional Search element in the admin section.
- **Swagger**. You can explore all endpoints and try to call them (with ApiKey Auth support).
- **Dependency injection**. I also used the `Scrutor` library for the register decorator (`CachedMovieSearchProvider` is a decorator of `MovieSearchProvider`) and for a scan of all `IMovieSource` implementations.
- **Validation**. All request models are validated automatically with the `FluentValidation` library.
- **Caching**. Requests to the external API are cached for 10 minutes (configurable). Cache implementation is behind an interface and can be easily changed from In-Memory to Redis or something different, like the `BitFaster.Caching` library.
- **Perfomance Improvings**. For example, statistics (I called it Audit) fetches as a FireAndForget principle (without waiting for ack from db) for improving the performance. In the future, we can use a queue (or a channel) for statistics gathering. We can use some external libraries like `Hangfire`.
- **Pagination**. Methods, that can return a lot of entries, supports pagination to prevent too much loading.
- **MongoDB Migrations**. I use the `Mongo.Migration` library to ensure that all indexes were created in the database before running.
- **Polly**. I use the `Polly` library to prevent exceptions from occasional network failures with third-party API, like `Omdb`.
- **Mapster** for mapping objects between different layers.
- **Docker support**. You can easily build and run the service with Docker.
- **Problem Details**. In Controller can be whether DomainException or ValidationException. Auth errors occur 401 exception, ValidationException occures 400-status error, DomainException and all other occure 500-status error (without details). 


