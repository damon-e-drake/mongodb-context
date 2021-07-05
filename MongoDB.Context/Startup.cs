using System;
using Microsoft.Extensions.DependencyInjection;

namespace MongoDB.Context
{
  public static class Startup
  {

    public static IServiceCollection AddMongoContext<T>(this IServiceCollection services, MongoDbContextOptions options) where T : MongoDbContext
    {
      services.AddSingleton((T)Activator.CreateInstance(typeof(T), new object[] { options }));

      return services;
    }
  }
}
