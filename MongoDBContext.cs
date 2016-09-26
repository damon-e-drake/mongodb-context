using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Mongo.Repository {
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
  public class CollectionNameAttribute : Attribute {
    public string Name { get; private set; }
    public CollectionNameAttribute(string name) {
      Name = name;
    }
  }
  public class MongoDBContext : IDisposable {
    private static List<IMongoClient> Clients = new List<IMongoClient>();
    private static List<string> Contexts = new List<string>();
    private static List<string> Databases = new List<string>();

    public MongoDBContext(string Name) {
      if (!Contexts.Contains(Name)) {
        var cn = ConfigurationManager.ConnectionStrings[Name].ConnectionString;
        var url = new MongoUrl(cn);

        Contexts.Add(Name);
        Clients.Add(new MongoClient(cn));
        Databases.Add(url.DatabaseName);
      }

      var index = Contexts.FindIndex(x => x == Name);

      foreach (var prop in GetType().GetProperties()) {
        var t = prop.PropertyType;
        if (t.ToString().Contains("MongoCollectionSet")) {
          var instance = Activator.CreateInstance(t, new object[] { Clients[index], Databases[index] });
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
