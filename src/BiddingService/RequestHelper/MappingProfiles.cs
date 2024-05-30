
using AutoMapper;
using BiddingService.DTOs;
using BiddingService.Entities;
using Contracts;

namespace AuctionService.RequestHelper;

public class MappingProfiles : Profile
{
	public MappingProfiles()
	{
		CreateMap<Bid, BidDto>();
		CreateMap<Bid, BidPlaced>();
	}
}