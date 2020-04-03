using MongoDB.Driver;
using System;

namespace MongoDB.Context
{

  public class MongoDbContext : IDisposable
  {
    private bool disposed = false;

    public IMongoClient Client { get; private set; }
    public IMongoDatabase Database { get; private set; }

    public MongoDbContext(MongoDbContextOptions options)
    {
      ConnectToClient(options);
      RegisterCollections();
    }

    private void ConnectToClient(MongoDbContextOptions options)
    {
      Client = new MongoClient(options.ConnectionString);
      Database = Client.GetDatabase(options.DatabaseName);
    }

    private void RegisterCollections()
    {
      foreach (var prop in GetType().GetProperties())
      {
        var t = prop.PropertyType;
        if (t.ToString().Contains("MongoCollectionSet"))
        {
          var instance = Activator.CreateInstance(t, new object[] { Database });
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
