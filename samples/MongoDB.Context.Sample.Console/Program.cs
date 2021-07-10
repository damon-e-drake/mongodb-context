using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Context.Interfaces;
using MongoDB.Context.Mapping;
using System;
using System.Threading.Tasks;

namespace MongoDB.Context.Sample
{
  public static class Program
  {
    public static Task Main(string[] args)
    {
      using IHost host = CreateHostBuilder(args).Build();
      using IServiceScope scope = host.Services.CreateScope();

      var context = scope.ServiceProvider.GetRequiredService<SampleContext>();

      var user = context.UserDocuments.AddAsync(new UserDocument
      {
        FirstName = "Fred",
        LastName = "Flintstone",
        Email = "fred@bedrock.com",
        ModifiedAt = DateTime.UtcNow
      }).Result;

      return host.RunAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
      Host.CreateDefaultBuilder(args)
        .ConfigureServices((_, services) =>
          services.AddMongoContext<SampleContext>(opts =>
					{
            opts.ConnectionString = "mongodb://10.0.47.79:27017";
            opts.DatabaseName = "SampleContext";
					})
        );
     
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

    public SampleContext(MongoDbContextOptions options) : base(options)
    {

    }

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
	}
}
