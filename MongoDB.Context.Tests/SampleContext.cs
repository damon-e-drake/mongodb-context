﻿using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Context.Attributes;
using MongoDB.Context.Interfaces;
using System;

namespace MongoDB.Context.Tests
{

  [CollectionName("Users")]
  public class UserDocument : IMongoDbDocument
  {
    [BsonElement("id"), BsonId]
    public string ID { get; set; }

    [BsonElement("modifiedAt")]
    public DateTime ModifiedAt { get; set; }
  }

  public class BlogDocument : IMongoDbDocument
  {
    [BsonElement("id"), BsonId]
    public string ID { get; set; }
    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [BsonElement("modifiedAt")]
    public DateTime ModifiedAt { get; set; }
  }

  public class SampleContext : MongoDbContext
  {

    public MongoCollectionSet<UserDocument> UserDocuments { get; set; }

    public MongoCollectionSet<BlogDocument> BlogDocuments { get; set; }

    public SampleContext(MongoDbContextOptions options) : base(options)
    {
      
    }
  }
}
