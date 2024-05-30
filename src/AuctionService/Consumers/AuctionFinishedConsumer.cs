using AuctionService.Data;
using AuctionService.Entities;
using Contracts;
using MassTransit;

namespace AuctionService.Consumers;

public class AuctionFinishedConsumer : IConsumer<AuctionFinished>
{
	private readonly AuctionDbContext _dbContext;

	public AuctionFinishedConsumer(AuctionDbContext DbContext)
	{
		_dbContext = DbContext;
	}

	public async Task Consume(ConsumeContext<AuctionFinished> context)
	{
		var guidId = Guid.Parse(context.Message.AuctionId);
		var auction = await _dbContext.Auctions.FindAsync(guidId);

		var isItemSold = context.Message.ItemSold;

		if (isItemSold)
		{
			auction.Winner = context.Message.Winner;
			auction.SoldAmount = context.Message.Amount ?? 0;
		}

		auction.Status = auction.SoldAmount > auction.ReservePrice
																				? Status.Finished
																				: Status.ReserveNotMet;

		await _dbContext.SaveChangesAsync();
	}
}