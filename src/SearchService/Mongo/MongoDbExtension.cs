using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;
using Newtonsoft.Json;

namespace SearchService.Mongo;

public static class MongoDbExtension
{
	public static async void UseMongoDb(this WebApplication app, WebApplicationBuilder builder)
	{
		var defaultConnection = builder.Configuration.GetConnectionString("default");
		var mongoClientSetting = MongoClientSettings.FromConnectionString(defaultConnection);
		await DB.InitAsync("SearchDb", mongoClientSetting);

		await DB.Index<Item>()
						.Key(a => a.Make, KeyType.Text)
						.Key(a => a.Model, KeyType.Text)
						.Key(a => a.Year, KeyType.Text)
						.Key(a => a.Color, KeyType.Text)
						.Key(a => a.Mileage, KeyType.Text)
						.CreateAsync();

		var count = await DB.CountAsync<Item>();

		// seed if no data
		if (count > 0) return;

		// get current directory
		var currentDirectory = Directory.GetCurrentDirectory();
		var json = File.ReadAllText($"{currentDirectory}/Data/seed.json");

		var itemsData = JsonConvert.DeserializeObject<List<Item>>(json);

		await DB.SaveAsync(itemsData);
	}
}