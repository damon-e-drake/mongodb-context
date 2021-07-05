## MongoDbContext
MongoDbContext is a lite weight wrapper designed using an Entity Framework style approach to include multiple collections
under a single context. This wrapper is intended for simple CRUD operations using MongoDB. The context does expose the
underlying client, database, and collection for more advanced usage.

## Getting Started
Install the [Nuget](https://www.nuget.org/packages/MongoDB.Context) package.

```
PM> Install-Package MongoDB.Context -Version 5.0.0-beta-1
```
## Creating the Models

Models should inherit from ```IMongoDbContext``` and can optionally use attributes to specify the name of the collection. 
Property names can be mapped using the ```[BsonElement]``` attribute. 

```csharp
[CollectionName("Users")]
public class UserDocument : IMongoDbDocument {
  public string ID {get; set;}

  [BsonElement("firstName")]
  public string FirstName {get; set;}

  [BsonElement("email")]
  public string Email {get; set;}

  public DateTime ModifiedAt {get; set;}
}
```

## Creating the MongoDbContext
The context should inherit from ```MongoDbContext``` and use a constructor that takes ```MongoDbContextOptions```. Any
models should be included using ```IMongoCollection<T>```. Collections are automatically wired to be used with the 
MongoDbContext at startup.

```csharp
public class SampleContext : MongoDbContext 
{
  public IMongoCollection<UserDocument> UserDocuments {get; set;}

  public SampleContext(MongoDbContextOptions options) : base(options)
  {
  }
}
```

## Configuring Dependency Injection
The MongoDbContext is setup to be used as a singleton under the hood. This is the intended behavior when using the 
MongoDB driver.
```csharp
public class Startup
{

  public void ConfigureServices(IServiceCollection services)
  {
    services.AddMongoContext<SampleContext>(new MongoDbContextOptions(
      connectionString: Configuration.GetValue<string>("MongoOptions:ConnectionString"),
      databaseName: Configuration.GetValue<string>("MongoOptions:Database")
    ));
  }
}
```

## Using with Dependency Injection

```csharp
public class SomeUserService
{
  private readonly SampleContext _context;

  public SomeClass(SampleContext context)
  {
    _context = context;
  }

  public async Task<UserDocument> FindUserById(string id) {
    var user = await _context.UserDocuments.FindAsync(id);
    return user;
  }
}

```