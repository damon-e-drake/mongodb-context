using System;
using Xunit;

namespace MongoDB.Context.Tests {
  public class ContextTest {

    private SampleContext _context;

    public ContextTest() {
      _context = new SampleContext(new MongoDbContextOptions(connectionString: "mongodb://10.0.47.79:3306", databaseName: "ContextTest"));
    }

    [Fact(DisplayName = "Should have a UserDocument collection.")]
    public void CollectionNotNull() {
      var collection = _context.UserDocuments;

      Assert.NotNull(collection);
    }

    [Fact(DisplayName = "Should have 0 User Documents.")]
    public void CountUserDocuments() {
      var count = _context.UserDocuments.TotalDocuments;

      Assert.Equal(0, count);
    }

    [Fact(DisplayName = "Should add 2 User Documents.")]
    public void CanAdd() {
      _context.UserDocuments.Add(new UserDocument { ID = Guid.NewGuid().ToString(), ModifiedAt = DateTime.UtcNow });
      _context.UserDocuments.Add(new UserDocument {ID = Guid.NewGuid().ToString(), ModifiedAt = DateTime.UtcNow.AddDays(-1) });

      var count = _context.UserDocuments.TotalDocuments;
      Assert.Equal(2, count);
    }

  }
}
