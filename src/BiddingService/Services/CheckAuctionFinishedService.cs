

using BiddingService.Entities;
using BiddingService.Models;
using Contracts;
using MassTransit;
using MongoDB.Entities;

namespace AuctionService.Services;

public class CheckAuctionFinishedService : BackgroundService
{
	private readonly IServiceProvider _service;

	public CheckAuctionFinishedService(IServiceProvider service)
	{
		_service = service;
	}
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		stoppingToken.Register(() => Console.WriteLine("CheckAuctionFinishedService is stopping."));

		while (!stoppingToken.IsCancellationRequested)
		{
			Console.WriteLine("CheckAuctionFinishedService is running.");

			await CheckAuction(stoppingToken);

			await Task.Delay(5000, stoppingToken);
		}
	}

	private async Task CheckAuction(CancellationToken stoppingToken)
	{
		var FinishedAuctions = await DB.Find<Auction>()
																.Match(a => a.Finished == false)
																.Match(a => a.AuctionEnd < DateTime.UtcNow)
																.ExecuteAsync();

		var scope = _service.CreateScope();

		var publisher = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

		foreach (var auction in FinishedAuctions)
		{
			auction.Finished = true;
			await auction.SaveAsync(null, stoppingToken);

			var winningBid = await DB.Find<Bid>()
																.Match(b => b.AuctionId == auction.ID)
																.Match(b => b.BidStatus == BidStatus.Accepted)
																.Sort(b => b.Amount, Order.Descending)
																.Limit(1)
																.ExecuteFirstAsync();

			var auctionFinished = new AuctionFinished
			{
				ItemSold = winningBid != null,
				AuctionId = auction.ID,
				Amount = winningBid?.Amount,
				Seller = auction.Seller,
				Winner = winningBid?.Bidder,
			};

			await publisher.Publish(auctionFinished, stoppingToken);
		}
	}
}