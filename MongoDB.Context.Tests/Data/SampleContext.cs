using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Context.Attributes;
using MongoDB.Context.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace MongoDB.Context.Tests.Data
{

	public static class MockDataLoader
  {
    public static IEnumerable<T> LoadData<T>(string fileName)
    {
			using var sr = new StreamReader($"mock-data/{fileName}");
			var json = sr.ReadToEnd();
			return JsonConvert.DeserializeObject<IEnumerable<T>>(json);
		}
  }

	public static class MockDataLoader
  {
    public static IEnumerable<T> LoadData<T>(string fileName)
    {
      using (var sr = new StreamReader($"mock-data/{fileName}"))
      {
        var json = sr.ReadToEnd();
        return JsonConvert.DeserializeObject<IEnumerable<T>>(json);
      }
    }
  }

  [CollectionName("Users")]
  public class UserDocument : IMongoDbDocument
  {
    public string ID { get; set; }
    public DateTime ModifiedAt { get; set; }
  }

  public class BlogDocument : IMongoDbDocument
  {
    public string ID { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ModifiedAt { get; set; }
  }

  public class SampleContext : MongoDbContext
  {

    public IMongoContextCollection<UserDocument> UserDocuments { get; set; }

    public IMongoContextCollection<BlogDocument> BlogDocuments { get; set; }

    public SampleContext(MongoDbContextOptions options) : base(options)
    {
      SeedCollections();
    }

    private void SeedCollections()
    {
      UserDocuments.SeedData(MockDataLoader.LoadData<UserDocument>("mock-users.json"));
    }
  }
}
