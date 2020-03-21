using MongoDB.Bson;
using MongoDB.Context.Attributes;
using MongoDB.Driver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MongoDB.Context {
  public class MongoCollectionSet<T> : IEnumerable<T> {
    public IMongoCollection<T> Collection { get; private set; }

    public long TotalDocuments {
      get { return Collection.CountDocuments(FilterDefinition<T>.Empty); }
    }

    public MongoCollectionSet(MongoClient client, string Database) {
      Collection = client.GetDatabase(Database).GetCollection<T>(GetCollectionName());
    }

    public void Add(T obj) {
      Collection.InsertOne(obj);
    }

    public void Update(string ID, T obj) {
      Collection.ReplaceOne(new BsonDocument("_id", new BsonObjectId(new ObjectId(ID))), obj);
    }

    public void Remove(string ID) {
      Collection.DeleteOne(new BsonDocument("_id", new BsonObjectId(new ObjectId(ID))));
    }

    public T Find(string ID) {
      var id = new BsonObjectId(new ObjectId(ID));
      return Collection.Find(new BsonDocument("_id", id)).FirstOrDefault();
    }

    public IEnumerable<T> Where(Expression<Func<T, bool>> expr) {
      return Collection.AsQueryable().Where(expr).ToList();
    }

    public IEnumerable<T> Select(Expression<Func<T, T>> expr) {
      return Collection.AsQueryable().Select(expr);
    }

    public T FirstOrDefault(Expression<Func<T, bool>> expr) {
      return Collection.AsQueryable().FirstOrDefault(expr);
    }

    public async Task<IEnumerable<T>> ToListAsync() {
      return await Collection.AsQueryable().ToListAsync();
    }

    private string GetCollectionName() {
      var attr = typeof(T).GetCustomAttributes(typeof(CollectionNameAttribute), true).FirstOrDefault() as CollectionNameAttribute;
      if (attr != null) { return attr.Name; }

      return typeof(T).Name.ToLower();
    }

    public IEnumerator<T> GetEnumerator() {
      return Collection.AsQueryable().ToList().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }
  }
}