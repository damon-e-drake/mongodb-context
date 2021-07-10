namespace MongoDB.Context
{
	public class MongoDbContextOptions
  {
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }

    public MongoDbContextOptions()
    {

    }

    public MongoDbContextOptions(string connectionString, string databaseName)
    {
      ConnectionString = connectionString;
      DatabaseName = databaseName;
    }
  }
}
