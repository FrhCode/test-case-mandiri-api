
using AuctionService.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AuctionService.Data;

public static class AuctionDbContextExtension
{
	public static void UseMigration(this WebApplication app)
	{
		using var serviceScope = app.Services.CreateScope();

		var auctionDbContext = serviceScope.ServiceProvider.GetRequiredService<AuctionDbContext>();

		try
		{
			seedData(auctionDbContext);
		}
		catch (Exception ex)
		{
			Console.WriteLine($"An error occurred while migrating the database: {ex.Message}");
		}
	}

	private static void seedData(AuctionDbContext auctionDbContext)
	{
		auctionDbContext.Database.Migrate();

		if (auctionDbContext.Auctions.Any())
		{
			Console.WriteLine("Auctions data already seeded");
			return;
		}
		Console.WriteLine("Seeding auctions data");
		var currentDirectory = Directory.GetCurrentDirectory();
		var json = File.ReadAllText($"{currentDirectory}/Data/seed.json");

		var auctionsData = JsonConvert.DeserializeObject<List<Auction>>(json);

		var auctions = auctionsData;

		auctionDbContext.Auctions.AddRange(auctions);

		auctionDbContext.SaveChanges();
	}
}