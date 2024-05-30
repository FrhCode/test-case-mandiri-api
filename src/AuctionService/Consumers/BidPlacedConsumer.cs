using AuctionService.Data;
using Contracts;
using MassTransit;

namespace AuctionService.Consumers;

public class BidPlacedConsumer : IConsumer<BidPlaced>
{
	private readonly AuctionDbContext _dbContext;

	public BidPlacedConsumer(AuctionDbContext DbContext)
	{
		_dbContext = DbContext;
	}

	public async Task Consume(ConsumeContext<BidPlaced> context)
	{
		Console.WriteLine("BidPlacedConsumer: {0}", context.Message.AuctionId);
		Guid auctionId = Guid.Parse(context.Message.AuctionId);

		var auction = await _dbContext.Auctions.FindAsync(auctionId);

		var isHighestBid = auction.CurrentHighBid > context.Message.Amount;
		var isAcceptedBid = context.Message.BidStatus.Contains("Accepted");

		if (!isHighestBid && !isAcceptedBid)
		{
			return;
		}

		auction.CurrentHighBid = context.Message.Amount;
		await _dbContext.SaveChangesAsync();
	}
}