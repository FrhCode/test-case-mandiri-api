
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Consumer;

public class AuctionFinishedConsumer : IConsumer<AuctionFinished>
{
	public async Task Consume(ConsumeContext<AuctionFinished> context)
	{
		var auction = await DB.Find<Item>().OneAsync(context.Message.AuctionId);

		var isItemSold = context.Message.ItemSold;

		if (isItemSold)
		{
			auction.Winner = context.Message.Winner;
			auction.SoldAmount = context.Message.Amount ?? 0;
		}

		auction.Status = "Finished";
		await auction.SaveAsync();
	}
}