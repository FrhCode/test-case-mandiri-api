using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AuctionService.Entities;

public class Auction
{
	[Required]
	public Guid Id { get; set; }

	[Required]
	public int ReservePrice { get; set; }

	[Required]
	public string Seller { get; set; }

	public string Winner { get; set; }

	public int SoldAmount { get; set; }

	public int CurrentHighBid { get; set; }

	[Required]
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

	[Required]
	public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

	[Required]
	public DateTime AuctionEnd { get; set; }

	[Required]
	public Status Status { get; set; }

	[Required]
	public Item Item { get; set; }
}