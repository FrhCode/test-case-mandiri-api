using Contracts;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using NotificationService.Hubs;

namespace NotificationService.Consumers;

public class AuctionBidPlacedConsumer : IConsumer<BidPlaced>
{
	private readonly IHubContext<NotificationHubs> _hubContext;

	public AuctionBidPlacedConsumer(IHubContext<NotificationHubs> hubContext)
	{
		_hubContext = hubContext;
	}

	public async Task Consume(ConsumeContext<BidPlaced> context)
	{
		Console.WriteLine("BidPlacedConsumer Consume method called");
		await _hubContext.Clients.All.SendAsync("BidPlaced", context.Message);
		Console.WriteLine("BidPlacedConsumer Consume method finished");
	}
}