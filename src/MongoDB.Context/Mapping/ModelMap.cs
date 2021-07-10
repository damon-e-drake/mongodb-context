using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Context.Interfaces;

namespace MongoDB.Context.Mapping
{
  public class ModelMap<T> where T : IMongoDbDocument
  {
    public List<ModelMapRegistry<T>> Registry { get; private set; } = new List<ModelMapRegistry<T>>();

    public void ToCollectionName(string name)
    {
      Registry.Add(new ModelMapRegistry<T>
      {
        Type = "CollectionName",
        Name = name
      });
    }

    public void HasObjectId(Expression<Func<T, object>> property)
    {
      Registry.Add(new ModelMapRegistry<T>
      {
        Type = "ID",
        Property = property,
        Name = "_id",
        IsRequired = true
      });
    }

    public void Property(Expression<Func<T, object>> property, string name, bool isRequired = false)
    {
      Registry.Add(new ModelMapRegistry<T>
      {
        Type = "Property",
        Property = property,
        Name = name,
        IsRequired = isRequired
      });
    }

    public void CompileModel()
    {
      BsonClassMap.RegisterClassMap<T>(t =>
      {
        foreach (var m in Registry)
        {
          if (m.Type == "ID")
          {
            t.MapIdMember(m.Property)
              .SetElementName(m.Name)
              .SetIsRequired(m.IsRequired)
              .SetIdGenerator(StringObjectIdGenerator.Instance)
              .SetSerializer(new StringSerializer(BsonType.ObjectId));
          }
          else if (m.Type == "Property")
          {
            t.MapMember(m.Property).SetIsRequired(m.IsRequired).SetElementName(m.Name);
          }
        }
      });
    }
  }
}
