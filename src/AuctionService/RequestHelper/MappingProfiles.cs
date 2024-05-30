using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Contracts;

namespace AuctionService.RequestHelper;

public class MappingProfiles : Profile
{
	public MappingProfiles()
	{
		CreateMap<Auction, AuctionDto>()
			.ForMember(dest => dest.Make, opt => opt.MapFrom(src => src.Item.Make))
			.ForMember(dest => dest.Model, opt => opt.MapFrom(src => src.Item.Model))
			.ForMember(dest => dest.Year, opt => opt.MapFrom(src => src.Item.Year))
			.ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Item.Color))
			.ForMember(dest => dest.Mileage, opt => opt.MapFrom(src => src.Item.Mileage))
			.ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Item.ImageUrl))
			.ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
			.ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Item.Description));

		CreateMap<CreateAuctionDto, Auction>()
			.ForMember(dest => dest.Item, opt => opt.MapFrom(src => new Item
			{
				Make = src.Make,
				Model = src.Model,
				Year = src.Year,
				Color = src.Color,
				Mileage = src.Mileage,
				Description = src.Description
			}));

		CreateMap<UpdateAuctionDto, Auction>()
			.ForMember(dest => dest.Item, opt => opt.MapFrom(src => new Item
			{
				Make = src.Make,
				Model = src.Model,
				Year = src.Year,
				Color = src.Color,
				Mileage = src.Mileage,
				Description = src.Description
			}));

		CreateMap<Auction, AuctionCreated>().ForMember(dest => dest.Make, opt => opt.MapFrom(src => src.Item.Make))
																				.ForMember(dest => dest.Model, opt => opt.MapFrom(src => src.Item.Model))
																				.ForMember(dest => dest.Year, opt => opt.MapFrom(src => src.Item.Year))
																				.ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Item.Color))
																				.ForMember(dest => dest.Mileage, opt => opt.MapFrom(src => src.Item.Mileage))
																				.ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Item.ImageUrl))
																				.ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
																				.ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Item.Description));

		CreateMap<Auction, AuctionUpdated>().ForMember(dest => dest.Make, opt => opt.MapFrom(src => src.Item.Make))
																				.ForMember(dest => dest.Model, opt => opt.MapFrom(src => src.Item.Model))
																				.ForMember(dest => dest.Year, opt => opt.MapFrom(src => src.Item.Year))
																				.ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Item.Color))
																				.ForMember(dest => dest.Mileage, opt => opt.MapFrom(src => src.Item.Mileage))
																				.ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Item.Description));

		CreateMap<AuctionDto, AuctionCreated>();

	}
}