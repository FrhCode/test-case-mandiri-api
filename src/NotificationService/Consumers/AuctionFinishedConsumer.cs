using Contracts;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using NotificationService.Hubs;

namespace NotificationService.Consumers;

public class AuctionFinishedConsumer : IConsumer<AuctionFinished>
{
	private readonly IHubContext<NotificationHubs> _hubContext;

	public AuctionFinishedConsumer(IHubContext<NotificationHubs> hubContext)
	{
		_hubContext = hubContext;
	}

	public async Task Consume(ConsumeContext<AuctionFinished> context)
	{
		Console.WriteLine("AuctionFinishedConsumer Consume method called");
		await _hubContext.Clients.All.SendAsync("AuctionFinished", context.Message);
		Console.WriteLine("AuctionFinishedConsumer Consume method finished");
	}
}