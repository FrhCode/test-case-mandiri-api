using BiddingService.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;
using BiddingService.Models;
using BiddingService.DTOs;
using AutoMapper;
using MassTransit;
using Contracts;
using BiddingService.Services;

namespace BiddingService.Controllers;

[ApiController]
[Route("api/bids")]
public class BidController : ControllerBase
{
	private readonly IMapper _mapper;
	private readonly IPublishEndpoint _publishEndpoint;
	private readonly GrpcAuctionClient _grpcAuctionClient;

	public BidController(IMapper mapper, IPublishEndpoint publishEndpoint, GrpcAuctionClient grpcAuctionClient)
	{
		_mapper = mapper;
		_publishEndpoint = publishEndpoint;
		_grpcAuctionClient = grpcAuctionClient;
	}

	[Authorize]
	[HttpPost("{id}")]
	public async Task<ActionResult<BidDto>> PlaceBid(string id, [FromForm] PlaceBidDto dto)
	{
		var amount = dto.Amount;

		var auction = await DB.Find<Auction>().OneAsync(id);

		if (auction == null)
		{
			auction = await _grpcAuctionClient.GetAuction(id);

			if (auction == null)
				return NotFound();
		}

		if (auction.Seller == User.Identity.Name)
		{
			return BadRequest("You cannot bid on your own auction");
		}

		var bid = new Bid
		{
			Amount = amount,
			AuctionId = id,
			Bidder = User.Identity.Name,
		};

		if (auction.AuctionEnd < DateTime.UtcNow)
		{
			bid.BidStatus = BidStatus.Finished;
		}
		else
		{
			var hisghestBid = await DB.Find<Bid>()
			.Match(b => b.AuctionId == id)
			.Sort(b => b.Amount, Order.Descending)
			.Limit(1)
			.ExecuteFirstAsync();

			if (hisghestBid != null)
			{
				if (amount > auction.ReservePrice)
				{
					bid.BidStatus = BidStatus.Accepted;
				}
				else if (amount > hisghestBid.Amount)
				{
					bid.BidStatus = BidStatus.AcceptedBelowReserve;
				}
				else
				{
					bid.BidStatus = BidStatus.TooLow;
				}
			}
			else
			{
				if (amount > auction.ReservePrice)
				{
					bid.BidStatus = BidStatus.Accepted;
				}
				else if (amount < auction.ReservePrice)
				{
					bid.BidStatus = BidStatus.AcceptedBelowReserve;
				}
			}

		}



		await DB.SaveAsync(bid);

		var bidPlaced = _mapper.Map<BidPlaced>(bid);

		await _publishEndpoint.Publish(bidPlaced);
		var bidDto = _mapper.Map<BidDto>(bid);


		return Ok(bidDto);
	}

	[HttpGet("{auctionId}")]
	public async Task<ActionResult<List<BidDto>>> GetBids(string auctionId)
	{
		var bids = await DB.Find<Bid>()
		.Match(b => b.AuctionId == auctionId)
		.Sort(b => b.BidTime, Order.Descending)
		.ExecuteAsync();

		var bidDtos = _mapper.Map<List<BidDto>>(bids);

		return Ok(bidDtos);
	}

}