using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MongoDB.Context.Interfaces
{
  public interface IMongoCollectionSet<T> : IEnumerable<T>
  {
    IMongoCollection<T> Collection { get; }
    long TotalDocuments { get; }

    void Add(T obj);
    T Find(string ID);
    T FirstOrDefault(Expression<Func<T, bool>> expr);
    IEnumerator<T> GetEnumerator();
    void Remove(string ID);
    IEnumerable<T> Select(Expression<Func<T, T>> expr);
    Task<IEnumerable<T>> ToListAsync();
    void Update(string ID, T obj);


    IEnumerable<T> Where(Expression<Func<T, bool>> expr);
  }
}