using System;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDB.Context.Interfaces
{

  public interface IMongoDbDocument
  {
    [BsonElement("id"), BsonId]
    string ID { get; set; }

    [BsonElement("modifiedAt")]
    DateTime ModifiedAt { get; set; }
  }

}
