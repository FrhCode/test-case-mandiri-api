using System.Globalization;
using AuctionService;
using BiddingService.Entities;
using Grpc.Net.Client;

namespace BiddingService.Services;

public class GrpcAuctionClient
{
	private readonly IConfiguration _config;

	public GrpcAuctionClient(IConfiguration config)
	{
		_config = config;
	}

	public async Task<Auction> GetAuction(string id)
	{

		var channel = GrpcChannel.ForAddress(_config["GrpcServer:Auction"]);
		var Client = new GrpcAuction.GrpcAuctionClient(channel);

		var request = new GetAuctionRequest { Id = id };

		try
		{
			var reply = await Client.GetAuctionAsync(request);
			var auction = new Auction
			{
				ID = reply.Auction.Id,
				Seller = reply.Auction.Seller,
				ReservePrice = reply.Auction.ReservePrice,
				AuctionEnd = DateTime.ParseExact(reply.Auction.AuctionEnd, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture)
			};

			return auction;
		}
		catch (Exception ex)
		{

			throw new Exception(ex.Message);
		}
	}
}