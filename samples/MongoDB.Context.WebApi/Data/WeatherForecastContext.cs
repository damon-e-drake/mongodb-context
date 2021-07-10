using MongoDB.Context.Interfaces;
using MongoDB.Context.Mapping;

namespace MongoDB.Context.WebApi.Data
{
	public class WeatherForecastContext : MongoDbContext
	{
		public IMongoContextCollection<WeatherForecastDocument> Forecasts { get; set; }

		public WeatherForecastContext(MongoDbContextOptions options) : base(options)
		{

		}

		public override void OnModelConfiguring(ModelBuilder builder) => 
			builder.Collection<WeatherForecastDocument>(c =>
			{
				c.ToCollectionName("Forecasts");
				c.HasObjectId(k => k.Id);
			});
	}
}
