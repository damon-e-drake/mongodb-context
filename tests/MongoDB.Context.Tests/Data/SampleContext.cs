using System;
using System.Collections.Generic;
using System.IO;
using MongoDB.Context.Interfaces;
using MongoDB.Context.Mapping;
using Newtonsoft.Json;

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

  public class UserDocument : IMongoDbDocument
  {
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public DateTime ModifiedAt { get; set; }
  }

  public class BlogDocument : IMongoDbDocument
  {
    public string Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ModifiedAt { get; set; }
  }

  public class SampleContext : MongoDbContext
  {

    public IMongoContextCollection<UserDocument> UserDocuments { get; set; }

    public IMongoContextCollection<BlogDocument> BlogDocuments { get; set; }

		public SampleContext(MongoDbContextOptions options) : base(options) => SeedCollections();

		public void OnModelConfiguring(ModelBuilder builder)
		{
      builder.Collection<UserDocument>(m =>
      {
        m.ToCollectionName("Users");
        m.HasObjectId(k => k.Id);
        m.Property(x => x.FirstName, name: "firstName", isRequired: true);
        m.Property(x => x.LastName, name: "lastName", isRequired: true);
        m.Property(x => x.Email, name: "email", isRequired: true);
        m.Property(x => x.ModifiedAt, name: "modifiedAt");
      });

      builder.Collection<BlogDocument>(m =>
      {
        m.ToCollectionName("Blogs");
        m.HasObjectId(k => k.Id);
        m.Property(p => p.CreatedAt, name: "createdAt");
      });
      
		}

		private void SeedCollections() => UserDocuments.SeedData(MockDataLoader.LoadData<UserDocument>("mock-users.json"));
	}
}
