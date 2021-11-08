# eShopWebApi
Rest API example with single Products endpoint used for hiring process. Endpoint supports versioning. Concrete version is specified in the path like `http:\\localhost:5000\v1\products`.
Supported operations:
* Get all products - only v1 (without paging)
* Get all products with paging support - only v2
* Get product details by product id
* Update product description
For more information run project and look into swagger documentation on address `/swagger`.

Program is implemented with `.NET Core 3.1` (latest LTS version). Data access is realized via Entity Framework Core.

## How to run project
### From Visual Studio
Just open solution file in Visual Studio and run project. You need installed `localdb` to run application with default configuration. LocalDB can be installed with SQL Server Express or
with Visual Studio (is part of ASP.Net and web development workload). See [SQL Server LocalDB MSDN doc](https://docs.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb?view=sql-server-ver15).
for more details. Connection string to a different database or in-memory database can be used. See configuration section for more details.
Available profiles for `IIS Express` and command line hosting - `eShopWebApi`. Docker and `docker-compose` are not handled within VS and need to be managed from command line.

API will be started on address `http:\\localhost:{free-port}`, default web browser with swagger documentation on address `http:\\localhost:{free-port}\swagger` will be opened.

You can run unit tests from Visual Studio.

## With dotnet from commandline
You can use these commands from root of the project:
* build application: `dotnet build`
* start server: `dotnet run --project .\src\eShopWebApi\eShopWebApi.csproj`
* run tests: `dotnet test`

API will be started on address `http:\\localhost:{free-port}`. Look into the commandline for concrete address. Look into `http:\\localhost:{free-port}\swagger` for swagger documentation.

## With Docker
Docker environment is managed with `docker-compose`. See configuration file [docker-compose.yml](./docker-compose.yml) You need to install `Docker` and `docker-compose` tool.
Build runs two containers `eShopWebApi` with API endpoint and `sqlserver` with Microsoft SQL server.
You can manage environment from commandline with these commands from the root directory:
* build: `docker-compose build`
* start containers: `docker-compose up -d`
* stop and remove containers: `docker-compose down`

API will be started on address `http:\\localhost:5000`, look into swagger documentation on `http:\\localhost:5000\swagger` for more details.

> :warning: Docker configuration is simplified. Some confidential informations, like connection string etc, are stored as environment variables. Also there is no override for docker. This would have to be handled better in real production environment.

## Configuration
Program uses [Microsoft Configuration Providers](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-3.1). You can see all program configuration
options in [appsettings.json](./src/eShopWebApi/appsettings.json). You can override them with change for any default configuration provider. `docker-compose` overrides them via environmental variables.
Some usefull variables:
* `eShopWebApi:UseInMemoryApplicationDb` - if set program uses in-memory provider for EntityFramework Core.
* `eShopWebApi:ApplyDbMigrationIfNeeded` - if set program applies EF Core migrations on startup (if needed)
* `eShopWebApi:SeedDatabaseWithTestData` - if set program seeds database with testing data, but only when is `Products` table empty, otherwise is initialization skipped
* `eShopWebApi:PathToInitProductsJsonFile` - path to json file, that is used to seed `Products` table

> :warning: Switch to In-memory database is allowed only for demo purposes. I would never allow it in production.
> :warning: Some properties shouldn't be defined in appsettings.json file nor in environment variables but should be defined in some trusted secret store.

## Implementation details and limitations
* I've tried to follow Clean Architecture (Onion Architecture) for this project. 
* Generic repository with QuerySpecification is used to access data. In standard project, I would use some existing implementation like [ardalis/Specification](https://github.com/ardalis/Specification).
But this would be probably too much simplification for this task. So I've implemented very basic version of it with only required methods.
* I've decided to version API with version tag in the URL. I think version in URL parameter fits better to REST API practices (URL is not changed with new version). But this method
is used quite often and it is more readable and understandable from swagger documentation (without too much customization).
* For paging I used parameters offset and limit. It is understandable and easy to implement. I've put paging information metadata into response headers instead of body. I like it more
because data and metadata are separated and for more reasons. Normally I would discuss it with customer or Rest API client and I would implement version that suits them better.

### Known Limitations
* In-memory database can be used in production code not just in tests. Normally I would never make in-memory DB part of production code. I've implemented it only because of my
interpretation of specification (might be useful for demo purposes).
* Some configuration, like connection string etc, should be configured via some secure storage not with env. variables. I've chosen this implementation for simplicity and because
secure storage or other security requirements were not specified.
* `docker-compose` configuration is very simple and basic. It would require some improvements for production (secure storage, overrides, kustomization, build optimization, ...).
* `docker-compose` environment works only with http (https not working). It was not in requirements. But https should be configured for production. Now https works only from commandline or Visual Studio.
* `docker-compose` don't have profiles in Visual Studio. I've run into some issues and I personally prefer working with these tools from command line.
* Github Actions or other build pipeline is not implemented for this project. It was not part of the specification.
* I've implemented only test useful for implementation. Page funkcionality can be easilly covered with combination of devtesting and unit test.
But reall production code would require better test coverage.