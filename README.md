# <img src="src/AgileTea.Persistence.Mongo/NugetIcon.png" alt="drawing" height="30"/> AgileTea.DocumentDb.Persistence

[![Build Status](https://agiletea.visualstudio.com/Agile%20Tea%20Document%20Db%20Persistence/_apis/build/status/agiletea.AgileTea.DocumentDb.Persistence?branchName=master)]()
[![NuGet Version](https://img.shields.io/nuget/v/AgileTea.Persistence.Mongo)](https://www.nuget.org/packages/AgileTea.Persistence.Mongo/)

Setup code for accessing  a document based database through repository-based code. Includes specific set up for [MongoDb][0].

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
    map => map.MapMember(x => x.SomeOtherProperty).SetIsRequired(true)
  .RegisterMongo();

  // ...
}
```

## Repository Methods

- GetAllAsync()
- GetByIdAsync()
- Add(TDocument document) // needs to be called within a unit of work and then committed to persist
- Update(TDocument document) // needs to be called within a unit of work and then committed to persist
- Remove(Guid id) // needs to be called within a unit of work and then committed to persist

## Usage

Inject an IRepository<T> into your class for use:
```csharp
[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
public class SomeClassController : ControllerBase
{
    private readonly IRepository<SomeClass> someClassRepository;

    public SomeClassController(IRepository<SomeClass> someClassRepository)
    {
        this.someClassRepository = someClassRepository;
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
        using var unitOfWork = new UnitOfWork(someClassRepository);
        someClassRepository.Add(SomeClass);
        await unitOfWork.CommitAsync();
        return Accepted();
    }

    [HttpPut]
    public async Task<ActionResult> UpdateAsync([FromBody]SomeClass SomeClass)
    {
        using var unitOfWork = new UnitOfWork(someClassRepository);
        someClassRepository.Update(SomeClass);
        await unitOfWork.CommitAsync().ConfigureAwait(false);
        return Accepted();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAsync(Guid id)
    {
        using var unitOfWork = new UnitOfWork(someClassRepository);
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
