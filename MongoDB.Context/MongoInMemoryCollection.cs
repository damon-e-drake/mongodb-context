using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Context.Attributes;
using MongoDB.Context.Interfaces;
using MongoDB.Driver;

namespace MongoDB.Context
{
  public class MongoInMemoryCollection<T> : IMongoContextCollection<T> where T : IMongoDbDocument
  {
    private readonly ConcurrentBag<T> MemoryCollection;

    public string CollectionName { get; private set; }

    public IMongoCollection<T> Collection { get; private set; }

    public long TotalDocuments => MemoryCollection.Count;

    public MongoInMemoryCollection(IMongoDatabase database = null)
    {
      MemoryCollection = new ConcurrentBag<T>();
      CollectionName = GetCollectionName();
    }

    private string GetCollectionName()
    {
      if (typeof(T).GetCustomAttributes(typeof(CollectionNameAttribute), true).FirstOrDefault() is CollectionNameAttribute attr) { return attr.Name; }

      return typeof(T).Name;
    }

    public Task<T> AddAsync(T item, InsertOneOptions opts = null)
    {
      item.ID = ObjectId.GenerateNewId().ToString();
      try
      {
        MemoryCollection.Add(item);
        return Task.FromResult(item);
      }
      catch
      {
        throw;
      }

    }

    public Task<T> FindAsync(string ID)
    {
      throw new NotImplementedException();
    }

    public Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> expr)
    {
      throw new NotImplementedException();
    }

    public IEnumerator<T> GetEnumerator()
    {
      throw new NotImplementedException();
    }

    public Task<bool> RemoveAsync(string ID)
    {
      throw new NotImplementedException();
    }

    public IEnumerable<T> Select(Expression<Func<T, T>> expr)
    {
      throw new NotImplementedException();
    }

    public Task<IEnumerable<T>> ToListAsync()
    {
      throw new NotImplementedException();
    }

    public Task<T> UpdateAsync(string ID, T item, ReplaceOptions opts = null)
    {
      throw new NotImplementedException();
    }

    public IEnumerable<T> Where(Expression<Func<T, bool>> expr)
    {
      throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      throw new NotImplementedException();
    }
  }
}
