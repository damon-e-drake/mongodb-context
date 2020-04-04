using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace MongoDB.Context.Interfaces
{

  public interface IMongoDbDocument
  {
    [BsonId]
    [BsonElement("_id")]
    [BsonRepresentation(BsonType.ObjectId)]
    string ID { get; set; }

    [BsonElement("modifiedAt")]
    DateTime ModifiedAt { get; set; }
  }

}
