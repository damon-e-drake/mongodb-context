using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Context.Attributes;
using MongoDB.Context.Interfaces;
using System;

namespace MongoDB.Context.Tests
{

  [CollectionName("Users")]
  public class UserDocument : IMongoDbDocument
  {
    [BsonId]
    [BsonElement("_id")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string ID { get; set; }

    [BsonElement("modifiedAt")]
    public DateTime ModifiedAt { get; set; }
  }

  public class BlogDocument : IMongoDbDocument
  {
    [BsonId]
    [BsonElement("_id")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string ID { get; set; }
    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [BsonElement("modifiedAt")]
    public DateTime ModifiedAt { get; set; }
  }

  public class SampleContext : MongoDbContext
  {

    public IMongoContextCollection<UserDocument> UserDocuments { get; set; }

    public IMongoContextCollection<BlogDocument> BlogDocuments { get; set; }

    public SampleContext(MongoDbContextOptions options) : base(options)
    {

    }
  }
}
