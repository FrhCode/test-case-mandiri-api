using AuctionService.Entities;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Data;

public class AuctionDbContext : DbContext
{
	public AuctionDbContext(DbContextOptions<AuctionDbContext> options) : base(options)
	{

	}

	public DbSet<Auction> Auctions { get; set; }
	public DbSet<Item> Items { get; set; }

	// onmodelcreating
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		// bus outbox and inbox
		modelBuilder.AddInboxStateEntity();
		modelBuilder.AddOutboxMessageEntity();
		modelBuilder.AddOutboxStateEntity();
	}
}