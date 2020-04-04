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
    private readonly ConcurrentBag<T> _collection;

    public string CollectionName { get; private set; }

    public IMongoCollection<T> Collection { get; private set; }

    public long TotalDocuments => _collection.Count;

    public MongoInMemoryCollection(IMongoDatabase database = null)
    {
      _collection = new ConcurrentBag<T>();
      CollectionName = GetCollectionName();
    }

    private string GetCollectionName()
    {
      if (typeof(T).GetCustomAttributes(typeof(CollectionNameAttribute), true).FirstOrDefault() is CollectionNameAttribute attr) { return attr.Name; }

      return typeof(T).Name;
    }

    public async Task<T> AddAsync(T item, InsertOneOptions opts = null)
    {
      item.ID = string.IsNullOrEmpty(item.ID) ? ObjectId.GenerateNewId().ToString() : item.ID;
      try
      {
        _collection.Add(item);
        return Task.FromResult(item);
      }
      catch
      {
        throw;
      }

    }

    public Task<T> FindAsync(string id)
    {
     return Task.FromResult(_collection.FirstOrDefault(x => x.ID == id));
    }

    public Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> expr)
    {
      throw new NotImplementedException();
    }

    public IEnumerator<T> GetEnumerator()
    {
      throw new NotImplementedException();
    }

    public async Task<bool> RemoveAsync(string id)
    {
      return await Task.Run(() =>
      {
        var item = _collection.FirstOrDefault(x => x.ID == id);
        if (item == null) { return false; }

        if (_collection.TryTake(out item)) { return true; }

        return false;
      }).ConfigureAwait(false);    
    }

    public IEnumerable<T> Select(Expression<Func<T, T>> expr)
    {
      throw new NotImplementedException();
    }

    public Task<IEnumerable<T>> ToListAsync()
    {
      throw new NotImplementedException();
    }

    public async Task<T> UpdateAsync(string id, T item, ReplaceOptions opts = null)
    {
      return await Task.Run(async () =>
      {
        var removed = await RemoveAsync(id).ConfigureAwait(false);

        if (removed) { AddAsync(item); }

        return default(T);
      }).ConfigureAwait(false);
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
