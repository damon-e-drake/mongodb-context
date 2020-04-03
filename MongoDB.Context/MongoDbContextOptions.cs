namespace MongoDB.Context
{
  public class MongoDbContextOptions
  {
    public string ConnectionString { get; private set; }
    public string DatabaseName { get; private set; }

    public MongoDbContextOptions(string connectionString, string databaseName)
    {
      ConnectionString = connectionString;
      DatabaseName = databaseName;
    }
  }
}
