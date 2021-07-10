using System;
using System.Linq.Expressions;

namespace MongoDB.Context.Mapping
{
  public class ModelMapRegistry<T>
  {
    public string Type { get; set; }
    public Expression<Func<T, object>> Property { get; set; }
    public string Name { get; set; }
    public bool IsRequired { get; set; }
  }
}
