using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Context.Attributes;
using MongoDB.Context.Interfaces;
using MongoDB.Driver;

namespace MongoDB.Context
{
  public class MongoCollection<T> : IMongoContextCollection<T> where T : IMongoDbDocument
  {
    public IMongoCollection<T> Collection { get; private set; }
    public string CollectionName { get; private set; }

    public long TotalDocuments => Collection.CountDocuments(FilterDefinition<T>.Empty);

    public MongoCollection(IMongoDatabase database)
    {
      CollectionName = GetCollectionName();
      Collection = database?.GetCollection<T>(CollectionName);
    }

    private string GetCollectionName()
    {
      if (typeof(T).GetCustomAttributes(typeof(CollectionNameAttribute), true).FirstOrDefault() is CollectionNameAttribute attr) { return attr.Name; }

      return typeof(T).Name;
    }

    public async Task<T> AddAsync(T item, InsertOneOptions opts = null)
    {
      await Collection.InsertOneAsync(item, opts).ConfigureAwait(false);
      return await FindAsync(item.ID).ConfigureAwait(false);
    }

    public async Task<T> UpdateAsync(string id, T item, ReplaceOptions opts = null)
    {
      var results = await Collection.ReplaceOneAsync(x => x.ID == id, item, opts).ConfigureAwait(false);
      if (results.IsAcknowledged && results.MatchedCount == 1)
      {
        return item;
      }
      else
      {
        return default;
      }
    }

    public async Task<bool> RemoveAsync(string id)
    {
      var results = await Collection.DeleteOneAsync(x => x.ID == id).ConfigureAwait(false);
      return (results.IsAcknowledged && results.DeletedCount == 1);
    }

    public async Task<T> FindAsync(string id)
    {
      var results = await Collection.FindAsync(x => x.ID == id).ConfigureAwait(false);
      return await results.FirstOrDefaultAsync().ConfigureAwait(false);
    }

    public IEnumerable<T> Where(Expression<Func<T, bool>> expr)
    {
      return Collection.AsQueryable().Where(expr).ToList();
    }

    public IEnumerable<T> Select(Expression<Func<T, T>> expr)
    {
      return Collection.AsQueryable().Select(expr);
    }

    public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> expr)
    {
      var results = await Collection.FindAsync(expr).ConfigureAwait(false);
      return await results.FirstOrDefaultAsync().ConfigureAwait(false);
    }

    public async Task<IEnumerable<T>> ToListAsync()
    {
      return await Collection.AsQueryable().ToListAsync().ConfigureAwait(false);
    }

    public IEnumerator<T> GetEnumerator()
    {
      return Collection.AsQueryable().ToList().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }
}
