using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace MongoDB.Context.Interfaces
{
  public interface IMongoContextCollection<T> : IEnumerable<T>
  {
    string CollectionName { get; }
    IMongoCollection<T> Collection { get; }
    long TotalDocuments { get; }

    void SeedData(IEnumerable<T> data);

    Task<T> AddAsync(T item, InsertOneOptions opts = null);
    Task<T> FindAsync(string id);
    Task<bool> RemoveAsync(string id);
    Task<T> UpdateAsync(string id, T item, ReplaceOptions opts = null);

    Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> expr);

    IEnumerable<T> Where(Expression<Func<T, bool>> expr);
    Task<IEnumerable<T>> ToListAsync();

    new IEnumerator<T> GetEnumerator();
  }
}
