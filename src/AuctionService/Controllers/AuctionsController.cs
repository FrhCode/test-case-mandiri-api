using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuctionService.Service;

namespace AuctionService.Controllers;

[ApiController]
[Route("api/auctions")]
public class AuctionsController : ControllerBase
{
	private readonly IMapper _mapper;
	private readonly AuctionDbContext _context;
	private readonly IPublishEndpoint _publishEndpoint;
	private readonly IFileUploadService _fileUploadService;

	public AuctionsController(IMapper mapper, AuctionDbContext context, IPublishEndpoint publishEndpoint, IFileUploadService fileUploadService)
	{
		_mapper = mapper;
		_context = context;
		_publishEndpoint = publishEndpoint;
		_fileUploadService = fileUploadService;
	}

	[HttpGet]
	public async Task<ActionResult<IEnumerable<AuctionDto>>> GetAllAuctions()
	{
		var auctions = await _context.Auctions
																				.Include(item => item.Item)
																				.OrderBy(item => item.Item.Make)
																				.ToListAsync();

		var auctionsDtos = _mapper.Map<IEnumerable<AuctionDto>>(auctions);
		return Ok(auctionsDtos);
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id)
	{
		var auction = await _context.Auctions.Include(a => a.Item).FirstOrDefaultAsync(a => a.Id == id);
		if (auction == null)
		{
			return NotFound();
		}

		var auctionDto = _mapper.Map<AuctionDto>(auction);
		return Ok(auctionDto);
	}

	[Authorize]
	[HttpPost]
	public async Task<ActionResult<AuctionDto>> CreateAuction([FromForm] CreateAuctionDto createAuctionDto)
	{
		var auction = _mapper.Map<Auction>(createAuctionDto);

		var a = User.Identity;
		auction.Seller = User.Identity.Name;

		var imageUrl = await _fileUploadService.UploadFileAsync("upload", createAuctionDto.Image);

		auction.Item.ImageUrl = imageUrl;

		await _context.Auctions.AddAsync(auction);

		var auctionDto = _mapper.Map<AuctionDto>(auction);
		var auctionCreated = _mapper.Map<AuctionCreated>(auction);

		await _publishEndpoint.Publish(auctionCreated);

		await _context.SaveChangesAsync();

		return CreatedAtAction(nameof(GetAuctionById), new { id = auction.Id }, auctionDto);
	}

	[Authorize]
	[HttpPut("{id}")]
	public async Task<ActionResult<AuctionDto>> UpdateAuction(Guid id, [FromForm] UpdateAuctionDto auctionDto)
	{
		var auction = await _context.Auctions.Include(a => a.Item).FirstOrDefaultAsync(a => a.Id == id);
		if (auction == null)
			return NotFound();

		if (auction.Seller != User.Identity.Name)
			return Forbid();

		auction.Item.Make = auctionDto.Make ?? auction.Item.Make;
		auction.Item.Model = auctionDto.Model ?? auction.Item.Model;
		auction.Item.Year = auctionDto.Year ?? auction.Item.Year;
		auction.Item.Color = auctionDto.Color ?? auction.Item.Color;
		auction.Item.Mileage = auctionDto.Mileage ?? auction.Item.Mileage;

		var auctionUpdated = _mapper.Map<AuctionUpdated>(auction);
		await _publishEndpoint.Publish(auctionUpdated);

		await _context.SaveChangesAsync();

		return Ok();
	}

	[Authorize]
	[HttpDelete("{id}")]
	public async Task<ActionResult> DeleteAuction(Guid id)
	{
		var auction = await _context.Auctions.FirstOrDefaultAsync(a => a.Id == id);
		if (auction == null)
			return NotFound();

		if (auction.Seller != User.Identity.Name)
			return Forbid();

		_context.Auctions.Remove(auction);
		await _publishEndpoint.Publish(new AuctionDeleted { Id = auction.Id });
		await _context.SaveChangesAsync();

		return Ok();
	}
}