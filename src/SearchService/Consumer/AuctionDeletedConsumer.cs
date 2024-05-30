using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Consumer;

public class AuctionDeletedConsumer : IConsumer<AuctionDeleted>
{


	public async Task Consume(ConsumeContext<AuctionDeleted> context)
	{
		Console.WriteLine($"AuctionDeletedConsumer: {context.Message.Id}");

		var item = await DB.Find<Item>().OneAsync(context.Message.Id);

		await item.DeleteAsync();
	}
}