using AuctionService.Data;
using Grpc.Core;

namespace AuctionService.Service;

public class GrpcAuctionService : GrpcAuction.GrpcAuctionBase
{
	private readonly AuctionDbContext _dbContext;

	public GrpcAuctionService(AuctionDbContext dbContext)
	{
		_dbContext = dbContext;
	}

    public override async Task<GrpcAuctionResponse> GetAuction(GetAuctionRequest request, ServerCallContext context)
    {
        var auction = await _dbContext.Auctions.FindAsync(Guid.Parse(request.Id));

        if (auction == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Auction not found"));
        }

        var response = new GrpcAuctionResponse
        {
            Auction = new GrpcAuctionModel
            {
                AuctionEnd = auction.AuctionEnd.ToString(),
                Id = auction.Id.ToString(),
                Seller = auction.Seller,
                ReservePrice = auction.ReservePrice,
            }
        };

        return response;
    }

    //public override async Task<GrpcAuctionResponse> GetAuction(GetAuctionRequest request)
    //{
    //	
    //}
}