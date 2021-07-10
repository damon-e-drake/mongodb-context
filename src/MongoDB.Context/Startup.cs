using System;
using Microsoft.Extensions.DependencyInjection;

namespace MongoDB.Context
{
	public static class Startup
  {

    public static IServiceCollection AddMongoContext<T>(this IServiceCollection services, Action<MongoDbContextOptions> options) where T : MongoDbContext
    {
      var dbOptions = new MongoDbContextOptions();
      options(dbOptions);

      services.AddSingleton((T)Activator.CreateInstance(typeof(T), new object[] { dbOptions }));

      return services;
    }
  }
}
