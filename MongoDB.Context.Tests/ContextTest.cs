using System;
using System.Threading.Tasks;
using Xunit;

namespace MongoDB.Context.Tests
{
  public class ContextTest
  {

    private readonly SampleContext _context;

    public ContextTest()
    {
      _context = new SampleContext(new MongoDbContextOptions(connectionString: "in-memory", databaseName: "ContextTest"));
      // _context.Database.DropCollection("Users");
    }

    [Fact(DisplayName = "In Memory should have null Client and Database")]
    public void ContextInMemory()
    {
      Assert.Null(_context.Database);
      Assert.Null(_context.Client);
    }

    [Fact(DisplayName = "Should have instanced collections.")]
    public void CollectionNotNull()
    {
      var users = _context.UserDocuments;
      var blogs = _context.BlogDocuments;

      Assert.NotNull(users);
      Assert.NotNull(blogs);
    }

    [Fact(DisplayName = "Should retieve collection names")]
    public void CollectionNaming()
    {
      var users = _context.UserDocuments;
      var blogs = _context.BlogDocuments;

      Assert.Equal("Users", users.CollectionName);
      Assert.Equal("BlogDocument", blogs.CollectionName);
    }

    [Fact(DisplayName = "Should have 0 User Documents.")]
    public void CountUserDocuments()
    {
      var count = _context.UserDocuments.TotalDocuments;

      Assert.Equal(0, count);
    }

    [Fact(DisplayName = "Should add 2 User Documents.")]
    public async Task CanAdd()
    {
      _ = await _context.UserDocuments.AddAsync(new UserDocument { ModifiedAt = DateTime.UtcNow });
      _ = await _context.UserDocuments.AddAsync(new UserDocument { ModifiedAt = DateTime.UtcNow.AddDays(-1) });
      _ = await _context.BlogDocuments.AddAsync(new BlogDocument { CreatedAt = DateTime.UtcNow, ModifiedAt = DateTime.UtcNow });

      var count = _context.UserDocuments.TotalDocuments;
      Assert.Equal(2, count);
    }

  }
}
