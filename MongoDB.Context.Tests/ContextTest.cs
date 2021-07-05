using MongoDB.Context.Tests.Data;
using System;
using System.Threading.Tasks;
using Xunit;

namespace MongoDB.Context.Tests
{
	public class ContextTest
  {
    private readonly SampleContext _context;

    public ContextTest(SampleContext context)
    {
      _context = context;
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

		[Fact(DisplayName = "Should have 30 User Documents.")]
		public void CountUserDocuments()
		{
			var count = _context.UserDocuments.TotalDocuments;

			Assert.Equal(28, count);
		}

		[Fact(DisplayName = "Should add 2 User Documents, 1 Blog Document.")]
		public async Task CanAdd()
		{
			_ = await _context.UserDocuments.AddAsync(new UserDocument { ModifiedAt = DateTime.UtcNow });
			_ = await _context.UserDocuments.AddAsync(new UserDocument { ModifiedAt = DateTime.UtcNow.AddDays(-1) });
			_ = await _context.BlogDocuments.AddAsync(new BlogDocument { CreatedAt = DateTime.UtcNow, ModifiedAt = DateTime.UtcNow });

			var ucount = _context.UserDocuments.TotalDocuments;
			var bcount = _context.BlogDocuments.TotalDocuments;

			Assert.Equal(30, ucount);
			Assert.Equal(1, bcount);
		}

		[Theory(DisplayName = "Can Find Items")]
		[InlineData("5e88c948fc13ae3945000033", true)]
		[InlineData("5e88c948fc13ae394500004f", true)]
		[InlineData("7f22c948fc13ae394500004f", false)]
		public async Task CanFind(string id, bool expected)
		{
			var item = await _context.UserDocuments.FindAsync(id);
			var found = item != null;

			Assert.Equal(expected, found);
		}

		[Theory(DisplayName = "Can Delete Items")]
		[InlineData("5e88c948fc13ae3945000033", true)]
		[InlineData("5e88c948fc13ae394500004f", true)]
		public async Task CanDelete(string id, bool expected)
		{
			var deleted = await _context.UserDocuments.RemoveAsync(id);

			Assert.Equal(expected, deleted);
		}

	}
}
