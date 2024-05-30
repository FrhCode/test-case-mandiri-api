using System.ComponentModel.DataAnnotations;

namespace AuctionService.Entities;

public class Item
{
	[Required]
	public Guid Id { get; set; }

	[Required]
	public string Make { get; set; }

	[Required]
	public string Model { get; set; }

	[Required]
	public int? Year { get; set; }

	[Required]
	public string Color { get; set; }

	[Required]
	public int? Mileage { get; set; }

	[Required]
	public string ImageUrl { get; set; }

	[Required]
	public Auction Auction { get; set; }

	[Required]
	public Guid AuctionId { get; set; }

	[Required]
	public string Description { get; set; }
}