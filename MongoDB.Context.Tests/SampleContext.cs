using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Context.Attributes;
using MongoDB.Context.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MongoDB.Context.Tests {

  [CollectionName("Users")]
  public class UserDocument : IMongoDbDocument {
    [BsonElement("id"), BsonId]
    public string ID { get; set; }

    [BsonElement("modifiedAt")]
    public DateTime ModifiedAt { get; set; }
  }

  public class SampleContext : MongoDbContext {

    public MongoCollectionSet<UserDocument> UserDocuments { get; set; }

    public SampleContext(MongoDbContextOptions options) : base(options) {

    }
  }
}
