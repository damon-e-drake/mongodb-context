using MongoDB.Driver;
using System;

namespace MongoDB.Context.Interfaces
{
  public interface IMongoDBContext : IDisposable
  {
    IMongoClient Client { get; }
    IMongoDatabase Database { get; }
  }
}
