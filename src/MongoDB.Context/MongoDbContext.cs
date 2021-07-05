using System;
using MongoDB.Context.Interfaces;
using MongoDB.Driver;

namespace MongoDB.Context
{
  public class MongoDbContext : IMongoDBContext
  {
    private bool disposed;

    public IMongoClient Client { get; private set; }
    public IMongoDatabase Database { get; private set; }

    public MongoDbContext(MongoDbContextOptions options)
    {
      if (options == null) 
        throw new ArgumentNullException(nameof(options));

      ConnectToClient(options);
      RegisterCollections(options);
    }

    private void ConnectToClient(MongoDbContextOptions options)
    {
      if (string.IsNullOrWhiteSpace(options.ConnectionString))
        throw new ArgumentException("Missing a connection string", nameof(options));

      if (options.ConnectionString.ToUpperInvariant() != "IN-MEMORY")
      {
        Client = new MongoClient(options.ConnectionString);
        Database = Client.GetDatabase(options.DatabaseName);
      }
    }

    private void RegisterCollections(MongoDbContextOptions options)
    {
      foreach (var prop in GetType().GetProperties())
      {
        var t = prop.PropertyType;
        if (t.IsInterface && t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IMongoContextCollection<>))
        {
          var isMemory = options.ConnectionString.ToUpperInvariant() == "IN-MEMORY";

          var generic = t.GetGenericArguments();
          var classType = isMemory ? typeof(MongoInMemoryCollection<>) : typeof(MongoCollection<>);
          var constructed = classType.MakeGenericType(generic);
          var instance = isMemory ? Activator.CreateInstance(constructed) : Activator.CreateInstance(constructed, new object[] { Database });

          prop.SetValue(this, instance);
        }
      }
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposed)
      {
        if (disposing)
        {
          Database = null;
          Client = null;
        }

        foreach (var prop in GetType().GetProperties())
        {
          prop.SetValue(this, null);
        }

        disposed = true;
      }
    }

    ~MongoDbContext()
    {
      Dispose(false);
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }
  }
}
