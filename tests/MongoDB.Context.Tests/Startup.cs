﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Context.Tests.Data;
using System.IO;

namespace MongoDB.Context.Tests
{
	public class Startup
	{
		public IConfiguration Configuration { get; private set; }

		public void ConfigureServices(IServiceCollection services)
		{
			SetConfiguration();

			services.AddMongoContext<SampleContext>(new MongoDbContextOptions(
				connectionString: Configuration.GetValue<string>("MongoOptions:ConnectionString"),
				databaseName: Configuration.GetValue<string>("MongoOptions:Database")
			));
		}

		private void SetConfiguration()
		{
			var builder = new ConfigurationBuilder();

			Configuration = builder.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
				.Build();
		}
	}
}