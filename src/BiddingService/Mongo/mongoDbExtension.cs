using MongoDB.Driver;
using MongoDB.Entities;
using Newtonsoft.Json;

namespace SearchService.Mongo;

public static class MongoDbExtension
{
	public static async void UseMongoDb(this WebApplication app, WebApplicationBuilder builder)
	{
		var defaultConnection = builder.Configuration.GetConnectionString("default");
		var mongoClientSetting = MongoClientSettings.FromConnectionString(defaultConnection);
		await DB.InitAsync("BidsDb", mongoClientSetting);
	}
}