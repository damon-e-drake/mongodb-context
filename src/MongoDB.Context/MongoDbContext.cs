using System;
using System.Linq;
using MongoDB.Context.Interfaces;
using MongoDB.Context.Mapping;
using MongoDB.Driver;

namespace MongoDB.Context
{
	public class MongoDbContext : IMongoDBContext
  {
    private bool disposed;
    protected ModelBuilder Builder { get; private set; } = new ModelBuilder();

    public IMongoClient Client { get; private set; }
    public IMongoDatabase Database { get; private set; }

    public MongoDbContext(MongoDbContextOptions options)
    {
      if (options == null)
			{
				throw new ArgumentNullException(nameof(options));
			}

			ConfigureClassMaps();
      ConnectToClient(options);
      RegisterCollections(options);
    }

		public virtual void OnModelConfiguring(ModelBuilder builder)
		{

		}

    private void ConnectToClient(MongoDbContextOptions options)
    {
      if (string.IsNullOrWhiteSpace(options.ConnectionString))
			{
				throw new ArgumentException("Missing a connection string", nameof(options));
			}

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

          var generic = t.GetGenericArguments()[0];
          var classType = isMemory ? typeof(MongoInMemoryCollection<>) : typeof(MongoCollection<>);
          var constructed = classType.MakeGenericType(generic);
          var collectionName = GetCollectionName(generic.Name);
          var instance = isMemory ? Activator.CreateInstance(constructed, new object[] { collectionName }) : Activator.CreateInstance(constructed, new object[] { Database, collectionName });

          prop.SetValue(this, instance);
        }
      }
    }

    private void ConfigureClassMaps()
    {
      var methods = GetType().GetMethods();
      var configure = methods.FirstOrDefault(x => x.Name == "OnModelConfiguring" && x.IsPublic);
      if (configure != null)
			{
				configure.Invoke(this, new object[] { Builder });
			}
		}

    private string GetCollectionName(string name)
    {
      if (Builder.CollectionRegistry.ContainsKey(name))
			{
				return Builder.CollectionRegistry[name];
			}

			return null;
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
