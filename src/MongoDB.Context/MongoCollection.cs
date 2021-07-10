using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Context.Interfaces;
using MongoDB.Driver;

namespace MongoDB.Context
{
	/// <summary>
	/// Represents a mongo collection and provides basic LINQ support to manage the collection.
	/// </summary>
	/// <typeparam name="T">Represents the IMongoDocument to be used as the collection document.</typeparam>
	public class MongoCollection<T> : IMongoContextCollection<T> where T : IMongoDbDocument
  {
    /// <summary>
    /// Provides public access to the underlying Mongo Collection.
    /// </summary>
    public IMongoCollection<T> Collection { get; private set; }

    /// <summary>
    /// Name of the collection that was inferred from the class name or CollectionName attribute.
    /// </summary>
    public string CollectionName { get; private set; }

    /// <summary>
    /// Returns the total number of objects contained in the collection.
    /// </summary>
    public long TotalDocuments => Collection.CountDocuments(FilterDefinition<T>.Empty);

    /// <summary>
    /// Creates an instance of a collection retrieved from a mongo database.
    /// </summary>
    /// <param name="database">Reference to the database containing the collection.</param>
    public MongoCollection(IMongoDatabase database, string collectionName)
    {
      CollectionName = collectionName;
      Collection = database?.GetCollection<T>(CollectionName);
    }

    /// <summary>
    /// Not used in a MongoCollection. This is used for the In-Memory collection.
    /// </summary>
    /// <param name="data">Collection of documents</param>
    public void SeedData(IEnumerable<T> data)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Adds a document to the current collection.
    /// </summary>
    /// <param name="item">Model representing the document to add.</param>
    /// <param name="opts">Options for inserting one document.</param>
    /// <returns>The document added to the collection.</returns>
    public async Task<T> AddAsync(T item, InsertOneOptions opts = null)
    {
      await Collection.InsertOneAsync(item, opts).ConfigureAwait(false);
      return await FindAsync(item.Id).ConfigureAwait(false);
    }

    /// <summary>
    /// Updates a single document to the current collection using mongo replace.
    /// </summary>
    /// <param name="id">Unique string representation of the BSON ID.</param>
    /// <param name="item">Model representing the document to update.</param>
    /// <param name="opts">Options for replacing  a single document.</param>
    /// <returns>The updated document in the collection.</returns>
    public async Task<T> UpdateAsync(string id, T item, ReplaceOptions opts = null)
    {
      var results = await Collection.ReplaceOneAsync(x => x.Id == id, item, opts).ConfigureAwait(false);
      return (results.IsAcknowledged && results.MatchedCount == 1) ? item : default;
    }

    /// <summary>
    /// Removes a single document from the current collection.
    /// </summary>
    /// <param name="id">Unique string representation of the BSON ID.</param>
    /// <returns>True or false depending on the deletion status.</returns>
    public async Task<bool> RemoveAsync(string id)
    {
      var results = await Collection.DeleteOneAsync(x => x.Id == id).ConfigureAwait(false);
      return (results.IsAcknowledged && results.DeletedCount == 1);
    }

    /// <summary>
    /// Find a single document in the current collection based on the unique id.
    /// </summary>
    /// <param name="id">Unique string representation of the BSON ID.</param>
    /// <returns>The document or null if not found in the current collection.</returns>
    public async Task<T> FindAsync(string id)
    {
      var results = await Collection.FindAsync(x => x.Id == id).ConfigureAwait(false);
      return await results.FirstOrDefaultAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Finds documents meeting certain criteria.
    /// </summary>
    /// <param name="expr">LINQ expression to filter documents returned in the results.</param>
    /// <returns>A collection of documents meeting the expression criteria.</returns>
    public IEnumerable<T> Where(Expression<Func<T, bool>> expr) => Collection.AsQueryable().Where(expr).ToList();

    //TODO: Select should return a new view, not the same Document with nulls on non-selected Fields

    /// <summary>
    /// Finds a document matching the expression.
    /// </summary>
    /// <param name="expr">LINQ express to filter documents.</param>
    /// <returns>The first document found based on the expression.</returns>
    public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> expr)
    {
      var results = await Collection.FindAsync(expr).ConfigureAwait(false);
      return await results.FirstOrDefaultAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Gets all documents in the current collection.
    /// </summary>
    /// <returns>A collection of all documents.</returns>
    public async Task<IEnumerable<T>> ToListAsync() => await Collection.AsQueryable().ToListAsync().ConfigureAwait(false);

    public IEnumerator<T> GetEnumerator() => Collection.AsQueryable().ToList().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
  }
}
