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
    private readonly ConcurrentDictionary<string, T> _collection;

    public string CollectionName { get; private set; }

    public IMongoCollection<T> Collection { get; private set; }

    public long TotalDocuments => _collection.Count;

    public MongoInMemoryCollection()
    {
      _collection = new ConcurrentDictionary<string, T>();
      CollectionName = GetCollectionName();
    }

    private string GetCollectionName()
    {
      if (typeof(T).GetCustomAttributes(typeof(CollectionNameAttribute), true).FirstOrDefault() is CollectionNameAttribute attr)
        return attr.Name;

      return typeof(T).Name;
    }

    public void SeedData(IEnumerable<T> data)
    {
      if (data == null)
        throw new ArgumentNullException(nameof(data));

      foreach (var item in data)
        _collection.TryAdd(item.ID, item);
    }

    public async Task<T> AddAsync(T item, InsertOneOptions opts = null)
    {
      return await Task.Run(() =>
      {
        item.ID = string.IsNullOrEmpty(item.ID) ? ObjectId.GenerateNewId().ToString() : item.ID;
        try
        {
          _collection.TryAdd(item.ID, item);
          return item;
        }
        catch
        {
          throw;
        }
      }).ConfigureAwait(false);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "KeyNotFound means item does not exist in this context.")]
    public Task<T> FindAsync(string id)
    {
      return Task.Run(() =>
      {
        try
        {
          return _collection[id];
        }
        catch (KeyNotFoundException)
        {
          return default;
        }
      });
    }

    public async Task<T> UpdateAsync(string id, T item, ReplaceOptions opts = null)
    {
      return await Task.Run(async () =>
      {
        var removed = await RemoveAsync(id).ConfigureAwait(false);

        if (removed)
        {
          await AddAsync(item).ConfigureAwait(false);
          return item;
        }

        return default;
      }).ConfigureAwait(false);
    }

    public async Task<bool> RemoveAsync(string id)
    {
      return await Task.Run(() =>
      {
        var item = _collection[id];
        if (item == null)
          return false;

        if (_collection.TryRemove(id, out item))
          return true;

        return false;
      }).ConfigureAwait(false);
    }

    public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> expr)
    {
      return await Task.Run(() =>
      {
        var items = _collection.Values.AsEnumerable();
        return items.FirstOrDefault(expr.Compile());
      }).ConfigureAwait(false);
    }

    public Task<IEnumerable<T>> ToListAsync() => Task.Run(() => _collection.Values.AsEnumerable());

    public IEnumerable<T> Where(Expression<Func<T, bool>> expr)
    {
      var items = _collection.Values.AsEnumerable();
      return items.Where(expr.Compile());
    }

    public IEnumerator<T> GetEnumerator() => _collection.Values.AsEnumerable().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
  }
}
