using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Consumer;

public class BidPlacedConsumer : IConsumer<BidPlaced>
{
	public async Task Consume(ConsumeContext<BidPlaced> context)
	{
		Console.WriteLine("BidPlacedConsumer: {0}", context.Message.Id);
		var auction = await DB.Find<Item>().OneAsync(context.Message.AuctionId);

		var isHighestBid = auction.CurrentHighBid > context.Message.Amount;
		var isAcceptedBid = context.Message.BidStatus.Contains("Accepted");

		if (!isHighestBid && !isAcceptedBid)
		{
			return;
		}

		auction.CurrentHighBid = context.Message.Amount;
		await auction.SaveAsync();
	}
}