## MongoDb.Context

MongoDbContext is a lite weight wrapper designed using an Entity Framework style approach to include multiple collections
under a single context. This wrapper is intended for simple CRUD operations using MongoDB. The context does expose the
underlying client, database, and collection for more advanced usage.

## Getting Started

Install the [Nuget](https://www.nuget.org/packages/MongoDB.Context) package.

```
PM> Install-Package MongoDB.Context -Version 5.0.2
```

```
> dotnet add package MongoDB.Context --version 5.0.2
```

## Creating the Models

Models should inherit from `IMongoDbContext`.

```csharp
public class UserDocument : IMongoDbDocument {
  public string Id {get; set;}
  public string FirstName {get; set;}
  public string Email {get; set;}
  public DateTime ModifiedAt {get; set;}
}
```

## Creating the MongoDbContext

The context should inherit from `MongoDbContext` and use a constructor that takes `MongoDbContextOptions`. Any
models should be included using `IMongoCollection<T>`. Collections are automatically wired up at runtime to register collections.

```csharp
public class SampleContext : MongoDbContext
{
  public IMongoCollection<UserDocument> UserDocuments {get; set;}

  public SampleContext(MongoDbContextOptions options) : base(options)
  {
  }

  public override void OnModelConfiguring(ModelBuilder builder)
  {
    builder.Collection<UserDocument>(m =>
      {
        m.ToCollectionName("Users");
        m.HasObjectId(k => k.Id);
        m.Property(x => x.FirstName, name: "firstName", isRequired: true);
        m.Property(x => x.Email, name: "email", isRequired: true);
        m.Property(x => x.ModifiedAt, name: "modifiedAt");
      });
  }
}
```

## Configuring Dependency Injection

The `MongoDbContext` is setup to be used as a singleton under the hood. This is the intended behavior when using the
MongoDB driver.

```csharp
public class Startup
{

  public void ConfigureServices(IServiceCollection services)
  {
    services.AddMongoContext<SampleContext>(opts =>
    {
      opts.ConnectionString = Configuration.GetValue<string>("MongoOptions:ConnectionString");
      opts.DatabaseName = Configuration.GetValue<string>("MongoOptions:Database");
    }));
  }
}
```

## Using with Dependency Injection

```csharp
public class SomeUserService
{
  private readonly SampleContext _context;

  public SomeUserService(SampleContext context)
  {
    _context = context;
  }

  public async Task<UserDocument> FindUserById(string id) {
    return await _context.UserDocuments.FindAsync(id);
  }
}

```
