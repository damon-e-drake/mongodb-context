using System;
using MongoDB.Driver;

namespace MongoDB.Context.Interfaces
{
  public interface IMongoDBContext : IDisposable
  {
    IMongoClient Client { get; }
    IMongoDatabase Database { get; }
  }
}
