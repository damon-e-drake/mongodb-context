using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Context.Interfaces;

namespace MongoDB.Context.Mapping
{
	public class ModelBuilder
  {
    public Dictionary<string, string> CollectionRegistry { get; set; } = new Dictionary<string, string>();

    public void Collection<T>(Action<ModelMap<T>> map) where T : IMongoDbDocument
    {      
      var mapper = new ModelMap<T>();
      map(mapper);

      var collection = mapper.Registry.FirstOrDefault(x => x.Type == "CollectionName");
      if (collection != null)
			{
				CollectionRegistry.Add(typeof(T).Name, collection.Name);
			}

			mapper.CompileModel();
    }
  }
}
