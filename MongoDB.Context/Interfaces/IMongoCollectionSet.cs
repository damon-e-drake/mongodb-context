using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MongoDB.Context.Interfaces
{
  public interface IMongoCollectionSet<T> : IEnumerable<T>
  {
    string CollectionName { get; }
    IMongoCollection<T> Collection { get; }
    long TotalDocuments { get; }

    Task<T> AddAsync(T item, InsertOneOptions opts = null);
    Task<T> FindAsync(string ID);
    Task<bool> RemoveAsync(string ID);
    Task<T> UpdateAsync(string ID, T obj, ReplaceOptions opts = null);

    Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> expr);
    IEnumerable<T> Select(Expression<Func<T, T>> expr);
    IEnumerable<T> Where(Expression<Func<T, bool>> expr);
    Task<IEnumerable<T>> ToListAsync();


    new IEnumerator<T> GetEnumerator();
  }
}
