# <img src="src/AgileTea.Persistence.Mongo/NugetIcon.png" alt="drawing" height="30"/> AgileTea.DocumentDb.Persistence

[![Build Status](https://agiletea.visualstudio.com/Agile%20Tea%20Document%20Db%20Persistence/_apis/build/status/agiletea.AgileTea.DocumentDb.Persistence?branchName=master)]()
[![NuGet Version](https://img.shields.io/nuget/v/AgileTea.Persistence.Mongo)](https://www.nuget.org/packages/AgileTea.Persistence.Mongo/)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=agiletea_AgileTea.DocumentDb.Persistence&metric=alert_status)](https://sonarcloud.io/dashboard?id=agiletea_AgileTea.DocumentDb.Persistence)

Setup code for accessing  a document based database through repository-based code. Includes specific set up for [MongoDb][0].

## 2.0.0 BETA Released

Upgrade to dotnet 5.0 and addition of Mongo Db ObjectId based document/ repository in addition to the current GUID Id based document/ repository.
This fits inline with requirements that align more closely with using the Mongo Db ObjectId as an identifier instead of a GUID.

It also allows for a Timestamp based indexed entity to be used that allows the developer to access the created timestamp from the ObjectId as well as the option to set a timestamp on update to provide a LastUpdated timestamp as well.

```csharp
public class ExampleDocument : TimestampedIndexedEntityBase
{
    public string Title { get; set; } = default!;
}

public class ExampleDocumentRepository : ObjectIdDocumentRespositoryBase<ExampleDocument>
{
    // std ctor
}


public class ExampleUpdateService
{
    private readonly ExampleDocumentRepository repository;

    internal ExampleUpdateService(ExampleDocumentRespository repository)
    {
        this.repository = repository;
    }

    public async Task<ExampleDocument> CreateDocument(string title)
    {
        var document = new ExampleDocument
        {
            Title = title
        };

        document.SetTimestamp();

        using var unitOfWork = unitOfWorkFactory.CreateUnitOfWork(repository);
        repository.Add(document);
        await unitOfWork.CommitAsync();
        return document;
    }
}
```

## 1.1.0 Released

Suport for Cosmos Db added through the MongoDb wire protocol.

Fix applied to ensure Mongo conventions are applied before class mappings. Note this removes the need (and ability) to call the RegisterMongo() method on startup:

```csharp
  services.AddMongo(options =>
  {
    options.DbConnection = Configuration["mongo:dbConnection"];
    options.DbName = Configuration["mongo:dbName"];
  })
  .AddMappings<SomeClass>(
    map => map.MapMember(x => x.SomeProperty).SetIsRequired(true),
    map => map.MapMember(x => x.SomeOtherProperty).SetIsRequired(true);
 // .RegisterMongo; (NO LONGER NEEDED - REMOVE IN EXISTING CODE!)
```

## Installation

AgileTea.Persistence.Mongo installs through [NuGet][1] and requires [.NET Standard][2] >= [2.0][3].

```
PS> Install-Package AgileTea.Persistence.Mongo
```

## Configuration

### Create document entity class to inherit from **IndexedEntityBase**

```csharp

public class SomeClass : IndexedEntityBase
{
  public string SomeProperty { get; set; } = default!;
  public string SomeOtherProperty { get; set; } = default!;
}
```

### Create repository for document entity class
```csharp
internal class SomeClassRepository : DocumentRepositoryBase<SomeClass>, IRepository<SomeClass>
{
  public SomeClassRepository(IMongoContext context, ILoggerFactory loggerFactory)
    : base(context, loggerFactory.CreateLogger<SomeClassRepository>())
  {
  }
}
```
### Setup configuration within IServiceCollection

```csharp

public void ConfigureServices(IServiceCollection services)
{
  // service configuration etc.
  // ...
  
  services.AddMongo(options =>
  {
    options.DbConnection = Configuration["mongo:dbConnection"];
    options.DbName = Configuration["mongo:dbName"];
  })
  .AddMappings<SomeClass>(
    map => map.MapMember(x => x.SomeProperty).SetIsRequired(true),
    map => map.MapMember(x => x.SomeOtherProperty).SetIsRequired(true);

  // ...
}
```

### NEW from Preview release version 1.1.0-preview1.20200118.1

Enable support for Cosmos via the MongoDB wire portocol api. Note that enabling this option will disable transactionality as Session creation is currently not supported in Cosmos through the MongoDb ewire protocol.

```csharp

public void ConfigureServices(IServiceCollection services)
{
  // service configuration etc.
  // ...
  
  services.AddMongo(options =>
  {
    options.CanSupportCosmos = true;
    options.DbConnection = Configuration["mongo:dbConnection"];
    options.DbName = Configuration["mongo:dbName"];
  })
  .AddMappings<SomeClass>(
    map => map.MapMember(x => x.SomeProperty).SetIsRequired(true),
    map => map.MapMember(x => x.SomeOtherProperty).SetIsRequired(true);

  // ...
}
```

## Repository Methods

- GetAllAsync()
- GetByIdAsync()
- Add(TDocument document) // needs to be called within a unit of work and then committed to persist
- Update(TDocument document) // needs to be called within a unit of work and then committed to persist
- Remove(Guid id) // needs to be called within a unit of work and then committed to persist

## Unit of Work

For those methods requiring a transactional approahc (Add/Update/Delete), the call tot he methods should be wrapped inside a unit of work.
To create  unit of work, inject an IUnitOfWorkFacotry into your constructor and call its **CreateUnitOfWork** method which will return an **IUnitOfWork**. 
This allows for both the factory and the unit of work to be mocked within your unit tests.
To commit a collection of commands called on a repository, called the async method **CommitAsync**

## Usage

Inject an IRepository<T> into your class for use:
```csharp
[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
public class SomeClassController : ControllerBase
{
    private readonly IRepository<SomeClass> someClassRepository;
    private readonly IUnitOfWorkFactory unitOfWorkFactory;

    public SomeClassController(
        IRepository<SomeClass> someClassRepository,
        IUnitOfWorkFactory unitOfWorkFactory)
    {
        this.someClassRepository = someClassRepository;
        this.unitOfWorkFactory = unitOfWorkFactory;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SomeClass>>> GetAllAsync()
    {
        return new ActionResult<IEnumerable<SomeClass>>(await someClassRepository.GetAllAsync().ConfigureAwait(false));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SomeClass>> GetByIdAsync(Guid id)
    {
        return new ActionResult<SomeClass>(await someClassRepository.GetByIdAsync(id).ConfigureAwait(false));
    }

    [HttpPost]
    public async Task<ActionResult> AddAsync([FromBody]SomeClass SomeClass)
    {
        using var unitOfWork = unitOfWorkFactory.CreateUnitOfWork(someClassRepository);
        someClassRepository.Add(SomeClass);
        await unitOfWork.CommitAsync();
        return Accepted();
    }

    [HttpPut]
    public async Task<ActionResult> UpdateAsync([FromBody]SomeClass SomeClass)
    {
        using var unitOfWork = unitOfWorkFactory.CreateUnitOfWork(someClassRepository);
        someClassRepository.Update(SomeClass);
        await unitOfWork.CommitAsync().ConfigureAwait(false);
        return Accepted();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAsync(Guid id)
    {
        using var unitOfWork = unitOfWorkFactory.CreateUnitOfWork(someClassRepository);
        someClassRepository.Remove(id);
        await unitOfWork.CommitAsync().ConfigureAwait(false);
        return Accepted();
    }
}
```


[0]: https://www.mongodb.com/
[1]: https://www.nuget.org/packages/AgileTea.Persistence.Mongo
[2]: https://docs.microsoft.com/en-us/dotnet/standard/net-standard
[3]: https://github.com/dotnet/standard/blob/master/docs/versions/netstandard2.0.md
