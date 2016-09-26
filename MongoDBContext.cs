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

  public class MongoClientConnection {
    public string ConnectionString { get; set; }
    public string Database { get; set; }
    public IMongoClient Client { get; set; }
  }

  public class MongoDBContext : IDisposable {
    private static List<MongoClientConnection> ClientsConnections = new List<MongoClientConnection>();

    public MongoDBContext(string ConnectionString) {
      MongoUrl url = null;

      if (ConnectionString.Trim().IndexOf("name=", StringComparison.InvariantCultureIgnoreCase) == 0) {
        ConnectionString = ConfigurationManager.ConnectionStrings[ConnectionString.Substring(5).Trim()].ConnectionString;
        url = new MongoUrl(ConnectionString);
      }
      else {
        url = new MongoUrl(ConnectionString);
      }

      var cn = ClientsConnections.FirstOrDefault(x => x.ConnectionString == ConnectionString);
      if (cn == null) {
        cn = new MongoClientConnection { ConnectionString = ConnectionString, Database = url.DatabaseName, Client = new MongoClient(ConnectionString) };
        ClientsConnections.Add(cn);
      }

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
