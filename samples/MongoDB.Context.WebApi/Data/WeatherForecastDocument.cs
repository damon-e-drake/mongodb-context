using MongoDB.Context.Interfaces;

namespace MongoDB.Context.WebApi.Data
{
	public class WeatherForecastDocument : IMongoDbDocument
	{
		public string Id { get; set; }
	}
}
