using MongoDB.Context.Attributes;
using MongoDB.Driver;
using System;
using System.Linq;

namespace MongoDB.Context {

  public class MongoDbContext : IDisposable {
    private bool disposed = false;

    public IMongoClient Client { get; private set; }
    public IMongoDatabase Database { get; private set; }

    public MongoDbContext(MongoDbContextOptions options) {
      RegisterCollections();
    }

    private void RegisterCollections() {
      foreach (var prop in GetType().GetProperties()) {
        var t = prop.PropertyType;
        if (t.ToString().Contains("MongoCollectionSet")) {
          var instance = Activator.CreateInstance(t, new object[] { cn.Client, cn.Database });
          prop.SetValue(this, instance);
        }
      }
    }

    private string GetCollectionName<T>() {
      var attr = typeof(T).GetCustomAttributes(typeof(CollectionNameAttribute), true).FirstOrDefault() as CollectionNameAttribute;
      if (attr != null) { return attr.Name; }

      return typeof(T).Name.ToLower();
    }

    public void Dispose() {
      foreach (var prop in GetType().GetProperties()) {
        prop.SetValue(this, null);
      }
    }
  }

}