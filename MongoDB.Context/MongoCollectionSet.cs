using MongoDB.Context.Attributes;
using MongoDB.Context.Interfaces;
using MongoDB.Driver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MongoDB.Context
{
  public class MongoCollectionSet<T> : IMongoCollectionSet<T> where T : IMongoDbDocument
  {
    public IMongoCollection<T> Collection { get; private set; }
    public string CollectionName { get; private set; }

    public long TotalDocuments => Collection.CountDocuments(FilterDefinition<T>.Empty);

    public MongoCollectionSet(IMongoDatabase database)
    {
      CollectionName = GetCollectionName();
      Collection = database.GetCollection<T>(CollectionName);
    }

    private string GetCollectionName()
    {
      var attr = typeof(T).GetCustomAttributes(typeof(CollectionNameAttribute), true).FirstOrDefault() as CollectionNameAttribute;
      if (attr != null) { return attr.Name; }

      return typeof(T).Name;
    }

    public async Task<T> AddAsync(T item, InsertOneOptions opts = null)
    {
      await Collection.InsertOneAsync(item, opts);
      return await FindAsync(item.ID);
    }

    public async Task<T> UpdateAsync(string id, T item, ReplaceOptions opts = null)
    {
      var results = await Collection.ReplaceOneAsync(x => x.ID == id, item, opts);
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
      var results = await Collection.DeleteOneAsync(x => x.ID == id);
      return (results.IsAcknowledged && results.DeletedCount == 1);
    }

    public async Task<T> FindAsync(string id)
    {
      var results = await Collection.FindAsync(x => x.ID == id);
      return await results.FirstOrDefaultAsync();
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
      var results = await Collection.FindAsync(expr);
      return await results.FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<T>> ToListAsync()
    {
      return await Collection.AsQueryable().ToListAsync();
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